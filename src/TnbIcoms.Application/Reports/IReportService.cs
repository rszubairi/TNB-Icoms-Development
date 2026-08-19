using TnbIcoms.Application.Common;
using TnbIcoms.Application.Outages.Dtos;
using TnbIcoms.Application.Reports.Dtos;

namespace TnbIcoms.Application.Reports;

public interface IReportService
{
    Task<ApiResponse<List<OutageListItemDto>>> GenerateAsync(ReportFilterDto filter);
    Task<byte[]> ExportExcelAsync(ReportFilterDto filter);
    Task<byte[]> ExportPdfAsync(ReportFilterDto filter);
    Task<ApiResponse<List<SavedReportFilterDto>>> ListFavouritesAsync(int userId);
    Task<ApiResponse<SavedReportFilterDto>> SaveFavouriteAsync(int userId, SaveReportFilterRequestDto request);
    Task<ApiResponse<object>> DeleteFavouriteAsync(int userId, int savedReportFilterId);
}
