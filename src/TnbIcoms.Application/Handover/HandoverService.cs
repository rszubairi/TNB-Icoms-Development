using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Handover.Dtos;
using TnbIcoms.Domain.Entities.Handover;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Handover;

public class HandoverService : IHandoverService
{
    private readonly AppDbContext _dbContext;

    private static readonly string[] ShiftOrder = { "Morning", "Evening", "Night" };

    public HandoverService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<HandoverShiftDto>> GetOrCreateShiftAsync(DateTime shiftDate, string shiftType, int zoneId)
    {
        if (!ShiftOrder.Contains(shiftType))
        {
            return ApiResponse<HandoverShiftDto>.Fail("Shift type must be Morning, Evening, or Night.");
        }

        var date = shiftDate.Date;
        var shift = await _dbContext.HandoverShifts
            .Include(s => s.Entries)
            .Include(s => s.Zone)
            .FirstOrDefaultAsync(s => s.ShiftDate == date && s.ShiftType == shiftType && s.ZoneId == zoneId);

        if (shift is null)
        {
            shift = new HandoverShift
            {
                ShiftDate = date,
                ShiftType = shiftType,
                ZoneId = zoneId,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.HandoverShifts.Add(shift);
            await _dbContext.SaveChangesAsync();
            shift = await _dbContext.HandoverShifts.Include(s => s.Entries).Include(s => s.Zone).FirstAsync(s => s.ShiftId == shift.ShiftId);
        }

        return ApiResponse<HandoverShiftDto>.Ok(await MapShiftAsync(shift));
    }

    public async Task<ApiResponse<List<HandoverShiftSummaryDto>>> ListShiftsAsync(int zoneId, DateTime? dateStart, DateTime? dateEnd)
    {
        var query = _dbContext.HandoverShifts.Include(s => s.Entries).Where(s => s.ZoneId == zoneId);
        if (dateStart.HasValue) query = query.Where(s => s.ShiftDate >= dateStart.Value.Date);
        if (dateEnd.HasValue) query = query.Where(s => s.ShiftDate <= dateEnd.Value.Date);

        var shifts = await query.OrderByDescending(s => s.ShiftDate).ThenByDescending(s => s.ShiftType).ToListAsync();

        return ApiResponse<List<HandoverShiftSummaryDto>>.Ok(shifts.Select(s => new HandoverShiftSummaryDto
        {
            ShiftId = s.ShiftId,
            ShiftDate = s.ShiftDate,
            ShiftType = s.ShiftType,
            IsPassed = s.IsPassed,
            EntryCount = s.Entries.Count
        }).ToList());
    }

    public async Task<ApiResponse<object>> UpdateShiftControlAsync(int shiftId, UpdateShiftControlRequestDto request, int currentUserId)
    {
        var shift = await _dbContext.HandoverShifts.FirstOrDefaultAsync(s => s.ShiftId == shiftId);
        if (shift is null) return ApiResponse<object>.Fail("Shift not found.");
        if (shift.IsPassed) return ApiResponse<object>.Fail("This shift has already been passed and is locked.");

        shift.ControlManagerId = request.ControlManagerId;
        shift.SwitchEngineer1Id = request.SwitchEngineer1Id;
        shift.SwitchEngineer2Id = request.SwitchEngineer2Id;
        shift.DespatcherId = request.DespatcherId;
        shift.ControlAssistantId = request.ControlAssistantId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<HandoverEntryDto>> AddEntryAsync(int shiftId, AddEntryRequestDto request, int currentUserId)
    {
        var shift = await _dbContext.HandoverShifts.FirstOrDefaultAsync(s => s.ShiftId == shiftId);
        if (shift is null) return ApiResponse<HandoverEntryDto>.Fail("Shift not found.");
        if (shift.IsPassed) return ApiResponse<HandoverEntryDto>.Fail("This shift has already been passed and is locked.");
        if (!HandoverCategories.All.Contains(request.Category)) return ApiResponse<HandoverEntryDto>.Fail("Unknown category.");
        if (string.IsNullOrWhiteSpace(request.Description)) return ApiResponse<HandoverEntryDto>.Fail("Description is required.");

        var entry = new HandoverEntry
        {
            ShiftId = shiftId,
            Category = request.Category,
            Description = request.Description.Trim(),
            RelatedOutageId = request.RelatedOutageId,
            IsLocked = false,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.HandoverEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<HandoverEntryDto>.Ok(await MapEntryAsync(entry));
    }

    public async Task<ApiResponse<object>> DeleteEntryAsync(int entryId, int currentUserId)
    {
        var entry = await _dbContext.HandoverEntries.Include(e => e.Shift).FirstOrDefaultAsync(e => e.HandoverEntryId == entryId);
        if (entry is null) return ApiResponse<object>.Fail("Entry not found.");
        if (entry.IsLocked || entry.Shift?.IsPassed == true) return ApiResponse<object>.Fail("This entry is locked and cannot be removed.");

        _dbContext.HandoverEntries.Remove(entry);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS §5.11: "Pass Handover" copies open entries to the next shift and locks the current one.
    /// Morning -> Evening (same date), Evening -> Night (same date), Night -> Morning (next date).
    /// </summary>
    public async Task<ApiResponse<HandoverShiftDto>> PassHandoverAsync(int shiftId, int currentUserId)
    {
        var shift = await _dbContext.HandoverShifts.Include(s => s.Entries).FirstOrDefaultAsync(s => s.ShiftId == shiftId);
        if (shift is null) return ApiResponse<HandoverShiftDto>.Fail("Shift not found.");
        if (shift.IsPassed) return ApiResponse<HandoverShiftDto>.Fail("This shift has already been passed.");

        var currentIndex = Array.IndexOf(ShiftOrder, shift.ShiftType);
        var nextShiftType = ShiftOrder[(currentIndex + 1) % ShiftOrder.Length];
        var nextDate = nextShiftType == "Morning" && shift.ShiftType == "Night" ? shift.ShiftDate.AddDays(1) : shift.ShiftDate;

        var nextShift = await _dbContext.HandoverShifts
            .FirstOrDefaultAsync(s => s.ShiftDate == nextDate && s.ShiftType == nextShiftType && s.ZoneId == shift.ZoneId);
        if (nextShift is null)
        {
            nextShift = new HandoverShift
            {
                ShiftDate = nextDate,
                ShiftType = nextShiftType,
                ZoneId = shift.ZoneId,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.HandoverShifts.Add(nextShift);
            await _dbContext.SaveChangesAsync();
        }

        foreach (var entry in shift.Entries)
        {
            entry.IsLocked = true;
            _dbContext.HandoverEntries.Add(new HandoverEntry
            {
                ShiftId = nextShift.ShiftId,
                Category = entry.Category,
                Description = entry.Description,
                RelatedOutageId = entry.RelatedOutageId,
                IsLocked = false,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        shift.IsPassed = true;
        shift.PassedAt = DateTime.UtcNow;
        shift.PassedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        var reloaded = await _dbContext.HandoverShifts.Include(s => s.Entries).Include(s => s.Zone).FirstAsync(s => s.ShiftId == shiftId);
        return ApiResponse<HandoverShiftDto>.Ok(await MapShiftAsync(reloaded));
    }

    private async Task<Dictionary<int, string>> GetPersonnelNamesAsync()
    {
        return await _dbContext.AuthorisationPersonnel.ToDictionaryAsync(p => p.AuthorisationPersonnelId, p => p.FullName);
    }

    private async Task<HandoverShiftDto> MapShiftAsync(HandoverShift shift)
    {
        var personnelNames = await GetPersonnelNamesAsync();
        var passedByName = shift.PassedBy.HasValue
            ? await _dbContext.AppUsers.Where(u => u.UserId == shift.PassedBy.Value).Select(u => u.FullName).FirstOrDefaultAsync()
            : null;

        string? Name(int? id) => id.HasValue && personnelNames.TryGetValue(id.Value, out var n) ? n : null;

        var outageIds = shift.Entries.Where(e => e.RelatedOutageId.HasValue).Select(e => e.RelatedOutageId!.Value).Distinct().ToList();
        var outageNumbers = outageIds.Count > 0
            ? await _dbContext.Outages.Where(o => outageIds.Contains(o.OutageId)).ToDictionaryAsync(o => o.OutageId, o => o.OutageNumber)
            : new Dictionary<int, string>();

        var createdByIds = shift.Entries.Select(e => e.CreatedBy).Distinct().ToList();
        var createdByNames = createdByIds.Count > 0
            ? await _dbContext.AppUsers.Where(u => createdByIds.Contains(u.UserId)).ToDictionaryAsync(u => u.UserId, u => u.FullName)
            : new Dictionary<int, string>();

        return new HandoverShiftDto
        {
            ShiftId = shift.ShiftId,
            ShiftDate = shift.ShiftDate,
            ShiftType = shift.ShiftType,
            ZoneId = shift.ZoneId,
            ZoneName = shift.Zone?.ZoneName,
            ControlManagerId = shift.ControlManagerId,
            ControlManagerName = Name(shift.ControlManagerId),
            SwitchEngineer1Id = shift.SwitchEngineer1Id,
            SwitchEngineer1Name = Name(shift.SwitchEngineer1Id),
            SwitchEngineer2Id = shift.SwitchEngineer2Id,
            SwitchEngineer2Name = Name(shift.SwitchEngineer2Id),
            DespatcherId = shift.DespatcherId,
            DespatcherName = Name(shift.DespatcherId),
            ControlAssistantId = shift.ControlAssistantId,
            ControlAssistantName = Name(shift.ControlAssistantId),
            IsPassed = shift.IsPassed,
            PassedAt = shift.PassedAt,
            PassedByName = passedByName,
            Entries = shift.Entries.OrderBy(e => e.CreatedAt).Select(e => new HandoverEntryDto
            {
                HandoverEntryId = e.HandoverEntryId,
                Category = e.Category,
                Description = e.Description,
                RelatedOutageId = e.RelatedOutageId,
                RelatedOutageNumber = e.RelatedOutageId.HasValue && outageNumbers.TryGetValue(e.RelatedOutageId.Value, out var num) ? num : null,
                IsLocked = e.IsLocked,
                CreatedByName = createdByNames.TryGetValue(e.CreatedBy, out var cn) ? cn : string.Empty,
                CreatedAt = e.CreatedAt
            }).ToList()
        };
    }

    private async Task<HandoverEntryDto> MapEntryAsync(HandoverEntry entry)
    {
        var createdByName = await _dbContext.AppUsers.Where(u => u.UserId == entry.CreatedBy).Select(u => u.FullName).FirstOrDefaultAsync();
        string? outageNumber = null;
        if (entry.RelatedOutageId.HasValue)
        {
            outageNumber = await _dbContext.Outages.Where(o => o.OutageId == entry.RelatedOutageId.Value).Select(o => o.OutageNumber).FirstOrDefaultAsync();
        }

        return new HandoverEntryDto
        {
            HandoverEntryId = entry.HandoverEntryId,
            Category = entry.Category,
            Description = entry.Description,
            RelatedOutageId = entry.RelatedOutageId,
            RelatedOutageNumber = outageNumber,
            IsLocked = entry.IsLocked,
            CreatedByName = createdByName ?? string.Empty,
            CreatedAt = entry.CreatedAt
        };
    }
}
