using TnbIcoms.Application.Common;
using TnbIcoms.Application.OutageTypeRules.Dtos;

namespace TnbIcoms.Application.OutageTypeRules;

public interface IOutageTypeRuleService
{
    Task<ApiResponse<List<OutageTypeRuleDto>>> ListAsync();
    Task<ApiResponse<OutageTypeRuleDto>> CreateAsync(SaveOutageTypeRuleRequestDto request);
    Task<ApiResponse<OutageTypeRuleDto>> UpdateAsync(int ruleId, SaveOutageTypeRuleRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int ruleId);
}
