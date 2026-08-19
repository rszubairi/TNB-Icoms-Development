using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.LinkingLines.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.LinkingLines;

public class LinkingLineService : ILinkingLineService
{
    private readonly AppDbContext _dbContext;

    public LinkingLineService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<LinkingLineDto>>> ListAsync()
    {
        var pairs = await _dbContext.LinkingLines
            .Include(l => l.Equipment)
            .Include(l => l.LinkedEquipment)
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.LinkingLineId)
            .Select(l => Map(l))
            .ToListAsync();

        return ApiResponse<List<LinkingLineDto>>.Ok(pairs);
    }

    public async Task<ApiResponse<LinkingLineDto>> CreateAsync(CreateLinkingLineRequestDto request)
    {
        if (request.EquipmentId == request.LinkedEquipmentId)
        {
            return ApiResponse<LinkingLineDto>.Fail("An equipment cannot be linked to itself.");
        }

        var equipmentExist = await _dbContext.Equipment
            .CountAsync(e => (e.EquipmentId == request.EquipmentId || e.EquipmentId == request.LinkedEquipmentId) && e.IsActive);
        if (equipmentExist != 2)
        {
            return ApiResponse<LinkingLineDto>.Fail("Selected equipment does not exist.");
        }

        var duplicate = await _dbContext.LinkingLines.AnyAsync(l => l.IsActive &&
            ((l.EquipmentId == request.EquipmentId && l.LinkedEquipmentId == request.LinkedEquipmentId) ||
             (l.EquipmentId == request.LinkedEquipmentId && l.LinkedEquipmentId == request.EquipmentId)));
        if (duplicate)
        {
            return ApiResponse<LinkingLineDto>.Fail("This pair is already configured as linked.");
        }

        var pair = new LinkingLine
        {
            EquipmentId = request.EquipmentId,
            LinkedEquipmentId = request.LinkedEquipmentId,
            Remark = request.Remark,
            IsActive = true
        };

        _dbContext.LinkingLines.Add(pair);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(pair).Reference(p => p.Equipment).LoadAsync();
        await _dbContext.Entry(pair).Reference(p => p.LinkedEquipment).LoadAsync();

        return ApiResponse<LinkingLineDto>.Ok(Map(pair));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int linkingLineId)
    {
        var pair = await _dbContext.LinkingLines.FirstOrDefaultAsync(l => l.LinkingLineId == linkingLineId);
        if (pair is null)
        {
            return ApiResponse<object>.Fail("Not found.");
        }

        pair.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static LinkingLineDto Map(LinkingLine pair)
    {
        return new LinkingLineDto
        {
            LinkingLineId = pair.LinkingLineId,
            EquipmentId = pair.EquipmentId,
            EquipmentName = pair.Equipment?.EquipmentName,
            LinkedEquipmentId = pair.LinkedEquipmentId,
            LinkedEquipmentName = pair.LinkedEquipment?.EquipmentName,
            Remark = pair.Remark,
            IsActive = pair.IsActive
        };
    }
}
