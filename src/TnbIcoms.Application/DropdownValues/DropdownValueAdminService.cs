using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.DropdownValues.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.DropdownValues;

public class DropdownValueAdminService : IDropdownValueAdminService
{
    private readonly AppDbContext _dbContext;

    public DropdownValueAdminService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<DropdownValueDto>>> ListAsync(string category)
    {
        var values = await _dbContext.DropdownValues
            .Where(d => d.CategoryCode == category)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.ValueLabel)
            .Select(d => Map(d))
            .ToListAsync();

        return ApiResponse<List<DropdownValueDto>>.Ok(values);
    }

    public async Task<ApiResponse<DropdownValueDto>> CreateAsync(CreateDropdownValueRequestDto request)
    {
        if (!DropdownCategories.Categories.Any(c => c.Code == request.CategoryCode))
        {
            return ApiResponse<DropdownValueDto>.Fail("Unknown dropdown category.");
        }

        var label = request.ValueLabel.Trim();
        if (string.IsNullOrWhiteSpace(label))
        {
            return ApiResponse<DropdownValueDto>.Fail("Value is required.");
        }

        var code = ToValueCode(label);

        if (await _dbContext.DropdownValues.AnyAsync(d => d.CategoryCode == request.CategoryCode && d.ValueCode == code))
        {
            return ApiResponse<DropdownValueDto>.Fail("This value already exists in the category.");
        }

        var maxSortOrder = await _dbContext.DropdownValues
            .Where(d => d.CategoryCode == request.CategoryCode)
            .Select(d => (int?)d.SortOrder)
            .MaxAsync() ?? 0;

        var value = new DropdownValue
        {
            CategoryCode = request.CategoryCode,
            ValueCode = code,
            ValueLabel = label,
            ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode,
            SortOrder = maxSortOrder + 1,
            IsActive = true
        };

        _dbContext.DropdownValues.Add(value);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<DropdownValueDto>.Ok(Map(value));
    }

    public async Task<ApiResponse<DropdownValueDto>> UpdateAsync(int dropdownValueId, UpdateDropdownValueRequestDto request)
    {
        var value = await _dbContext.DropdownValues.FirstOrDefaultAsync(d => d.DropdownValueId == dropdownValueId);
        if (value is null)
        {
            return ApiResponse<DropdownValueDto>.Fail("Dropdown value not found.");
        }

        var label = request.ValueLabel.Trim();
        if (string.IsNullOrWhiteSpace(label))
        {
            return ApiResponse<DropdownValueDto>.Fail("Value is required.");
        }

        value.ValueLabel = label;
        value.ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode;
        value.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<DropdownValueDto>.Ok(Map(value));
    }

    public async Task<ApiResponse<object>> ReorderAsync(int dropdownValueId, ReorderDropdownValueRequestDto request)
    {
        var value = await _dbContext.DropdownValues.FirstOrDefaultAsync(d => d.DropdownValueId == dropdownValueId);
        if (value is null)
        {
            return ApiResponse<object>.Fail("Dropdown value not found.");
        }

        var siblings = await _dbContext.DropdownValues
            .Where(d => d.CategoryCode == value.CategoryCode)
            .OrderBy(d => d.SortOrder)
            .ToListAsync();

        var index = siblings.FindIndex(d => d.DropdownValueId == dropdownValueId);
        var targetIndex = request.Direction == "up" ? index - 1 : index + 1;

        if (targetIndex < 0 || targetIndex >= siblings.Count)
        {
            return ApiResponse<object>.Fail("Cannot move further in that direction.");
        }

        (siblings[index].SortOrder, siblings[targetIndex].SortOrder) = (siblings[targetIndex].SortOrder, siblings[index].SortOrder);

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int dropdownValueId)
    {
        var value = await _dbContext.DropdownValues.FirstOrDefaultAsync(d => d.DropdownValueId == dropdownValueId);
        if (value is null)
        {
            return ApiResponse<object>.Fail("Dropdown value not found.");
        }

        value.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static string ToValueCode(string label)
    {
        var cleaned = new string(label.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray());
        return cleaned.Replace(" ", "_").ToUpperInvariant();
    }

    private static DropdownValueDto Map(DropdownValue value)
    {
        return new DropdownValueDto
        {
            DropdownValueId = value.DropdownValueId,
            CategoryCode = value.CategoryCode,
            ValueCode = value.ValueCode,
            ValueLabel = value.ValueLabel,
            ParentCode = value.ParentCode,
            SortOrder = value.SortOrder,
            IsActive = value.IsActive
        };
    }
}
