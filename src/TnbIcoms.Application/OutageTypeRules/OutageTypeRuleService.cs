using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.OutageTypeRules.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.OutageTypeRules;

public class OutageTypeRuleService : IOutageTypeRuleService
{
    private readonly AppDbContext _dbContext;

    public OutageTypeRuleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<OutageTypeRuleDto>>> ListAsync()
    {
        var rules = await _dbContext.OutageTypeRules
            .OrderBy(r => r.WorkTypeCode)
            .ThenBy(r => r.OutageTypeCode)
            .Select(r => Map(r))
            .ToListAsync();

        return ApiResponse<List<OutageTypeRuleDto>>.Ok(rules);
    }

    public async Task<ApiResponse<OutageTypeRuleDto>> CreateAsync(SaveOutageTypeRuleRequestDto request)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ApiResponse<OutageTypeRuleDto>.Fail(validationError);
        }

        var existingRules = await _dbContext.OutageTypeRules
            .Where(r => r.IsActive && r.WorkTypeCode == request.WorkTypeCode)
            .ToListAsync();

        var conflict = FindConflict(request, existingRules, excludeRuleId: null);
        if (conflict is not null)
        {
            return ApiResponse<OutageTypeRuleDto>.Fail(conflict);
        }

        var rule = new OutageTypeRule
        {
            OutageTypeCode = request.OutageTypeCode,
            WorkTypeCode = request.WorkTypeCode,
            MoreThanDays = request.MoreThanDays,
            MoreThanMonths = request.MoreThanMonths,
            MoreThanYears = request.MoreThanYears,
            LessThanDays = request.LessThanDays,
            LessThanMonths = request.LessThanMonths,
            LessThanYears = request.LessThanYears,
            AppliesTo = NormalizeAppliesTo(request.AppliesTo),
            IsActive = true
        };

        _dbContext.OutageTypeRules.Add(rule);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<OutageTypeRuleDto>.Ok(Map(rule));
    }

    public async Task<ApiResponse<OutageTypeRuleDto>> UpdateAsync(int ruleId, SaveOutageTypeRuleRequestDto request)
    {
        var rule = await _dbContext.OutageTypeRules.FirstOrDefaultAsync(r => r.OutageTypeRuleId == ruleId);
        if (rule is null)
        {
            return ApiResponse<OutageTypeRuleDto>.Fail("Rule not found.");
        }

        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ApiResponse<OutageTypeRuleDto>.Fail(validationError);
        }

        var existingRules = await _dbContext.OutageTypeRules
            .Where(r => r.IsActive && r.WorkTypeCode == request.WorkTypeCode && r.OutageTypeRuleId != ruleId)
            .ToListAsync();

        var conflict = FindConflict(request, existingRules, excludeRuleId: ruleId);
        if (conflict is not null)
        {
            return ApiResponse<OutageTypeRuleDto>.Fail(conflict);
        }

        rule.OutageTypeCode = request.OutageTypeCode;
        rule.WorkTypeCode = request.WorkTypeCode;
        rule.MoreThanDays = request.MoreThanDays;
        rule.MoreThanMonths = request.MoreThanMonths;
        rule.MoreThanYears = request.MoreThanYears;
        rule.LessThanDays = request.LessThanDays;
        rule.LessThanMonths = request.LessThanMonths;
        rule.LessThanYears = request.LessThanYears;
        rule.AppliesTo = NormalizeAppliesTo(request.AppliesTo);

        await _dbContext.SaveChangesAsync();

        return ApiResponse<OutageTypeRuleDto>.Ok(Map(rule));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int ruleId)
    {
        var rule = await _dbContext.OutageTypeRules.FirstOrDefaultAsync(r => r.OutageTypeRuleId == ruleId);
        if (rule is null)
        {
            return ApiResponse<object>.Fail("Rule not found.");
        }

        rule.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static string? Validate(SaveOutageTypeRuleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.OutageTypeCode) || string.IsNullOrWhiteSpace(request.WorkTypeCode))
        {
            return "Outage type and work type are required.";
        }

        var moreDays = ToApproxDays(request.MoreThanDays, request.MoreThanMonths, request.MoreThanYears);
        var lessDays = ToApproxDays(request.LessThanDays, request.LessThanMonths, request.LessThanYears);

        if (moreDays == 0 && lessDays == 0)
        {
            return "At least one of the More Than / Less Than bounds is required.";
        }

        if (lessDays > 0 && moreDays >= lessDays)
        {
            return "The 'Less Than' bound must be greater than the 'More Than' bound.";
        }

        return null;
    }

    /// <summary>
    /// URS Module 1 §5.2.8: rules of the same Work Type with overlapping voltage coverage
    /// cannot claim overlapping lead-time windows.
    /// </summary>
    private static string? FindConflict(SaveOutageTypeRuleRequestDto request, List<OutageTypeRule> existingRules, int? excludeRuleId)
    {
        var (newMore, newLess) = GetRange(request.MoreThanDays, request.MoreThanMonths, request.MoreThanYears,
            request.LessThanDays, request.LessThanMonths, request.LessThanYears);
        var newVoltages = ParseAppliesTo(request.AppliesTo);

        foreach (var existing in existingRules)
        {
            if (excludeRuleId.HasValue && existing.OutageTypeRuleId == excludeRuleId.Value)
            {
                continue;
            }

            var existingVoltages = ParseAppliesTo(existing.AppliesTo);
            var voltagesOverlap = newVoltages is null || existingVoltages is null
                || newVoltages.Overlaps(existingVoltages);

            if (!voltagesOverlap)
            {
                continue;
            }

            var (existingMore, existingLess) = GetRange(existing.MoreThanDays, existing.MoreThanMonths, existing.MoreThanYears,
                existing.LessThanDays, existing.LessThanMonths, existing.LessThanYears);

            var rangesOverlap = newMore < existingLess && existingMore < newLess;
            if (rangesOverlap)
            {
                return $"This range conflicts with the existing {existing.OutageTypeCode} rule for {existing.WorkTypeCode} outages (AppliesTo: {existing.AppliesTo}).";
            }
        }

        return null;
    }

    private static (long More, long Less) GetRange(int? moreDays, int? moreMonths, int? moreYears, int? lessDays, int? lessMonths, int? lessYears)
    {
        var more = ToApproxDays(moreDays, moreMonths, moreYears);
        var lessRaw = ToApproxDays(lessDays, lessMonths, lessYears);
        var less = lessRaw > 0 ? lessRaw : long.MaxValue / 2;
        return (more, less);
    }

    private static long ToApproxDays(int? days, int? months, int? years)
    {
        return (days ?? 0) + (long)(months ?? 0) * 30 + (long)(years ?? 0) * 365;
    }

    private static HashSet<string>? ParseAppliesTo(string appliesTo)
    {
        if (string.IsNullOrWhiteSpace(appliesTo) || appliesTo.Trim().Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            return null; // null = matches every voltage
        }

        return appliesTo.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => v.ToUpperInvariant())
            .ToHashSet();
    }

    private static string NormalizeAppliesTo(string appliesTo)
    {
        return string.IsNullOrWhiteSpace(appliesTo) ? "ALL" : appliesTo.Trim();
    }

    private static OutageTypeRuleDto Map(OutageTypeRule rule)
    {
        return new OutageTypeRuleDto
        {
            OutageTypeRuleId = rule.OutageTypeRuleId,
            OutageTypeCode = rule.OutageTypeCode,
            WorkTypeCode = rule.WorkTypeCode,
            MoreThanDays = rule.MoreThanDays,
            MoreThanMonths = rule.MoreThanMonths,
            MoreThanYears = rule.MoreThanYears,
            LessThanDays = rule.LessThanDays,
            LessThanMonths = rule.LessThanMonths,
            LessThanYears = rule.LessThanYears,
            AppliesTo = rule.AppliesTo,
            IsActive = rule.IsActive
        };
    }
}
