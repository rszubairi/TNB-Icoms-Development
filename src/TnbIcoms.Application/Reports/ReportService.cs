using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Outages;
using TnbIcoms.Application.Outages.Dtos;
using TnbIcoms.Application.Reports.Dtos;
using TnbIcoms.Domain.Entities.Audit;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Reports;

public class ReportService : IReportService
{
    private readonly IOutageService _outageService;
    private readonly AppDbContext _dbContext;

    public ReportService(IOutageService outageService, AppDbContext dbContext)
    {
        _outageService = outageService;
        _dbContext = dbContext;
    }

    private static OutageListFilter ToOutageFilter(ReportFilterDto filter) => new()
    {
        ZoneId = filter.ZoneId,
        StationId = filter.StationId,
        JobTypeId = filter.JobTypeId,
        OutageCode = filter.OutageCode,
        RequestorStatus = filter.RequestorStatus,
        GnmStatus = filter.GnmStatus,
        Keyword = filter.Keyword,
        DateStart = filter.DateStart,
        DateEnd = filter.DateEnd,
        ShowDraft = filter.ShowDraft,
        SortBy = filter.SortBy
    };

    public async Task<ApiResponse<List<OutageListItemDto>>> GenerateAsync(ReportFilterDto filter)
    {
        return await _outageService.ListAsync(ToOutageFilter(filter));
    }

    public async Task<byte[]> ExportExcelAsync(ReportFilterDto filter)
    {
        var result = await _outageService.ListAsync(ToOutageFilter(filter));
        var rows = result.Data ?? new List<OutageListItemDto>();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Outage Report");

        string[] headers = { "Outage Number", "Type", "Zone", "Station", "Voltage", "Equipment", "Job Type", "Planned Start", "Planned End", "Requestor Status", "Planner Status", "GNM Status", "GNC Status", "Description" };
        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var row = 2;
        foreach (var o in rows)
        {
            sheet.Cell(row, 1).Value = o.OutageNumber;
            sheet.Cell(row, 2).Value = o.OutageTypeCode;
            sheet.Cell(row, 3).Value = o.ZoneName;
            sheet.Cell(row, 4).Value = o.StationName;
            sheet.Cell(row, 5).Value = o.VoltageLevelName;
            sheet.Cell(row, 6).Value = o.EquipmentName;
            sheet.Cell(row, 7).Value = o.JobTypeLabel;
            sheet.Cell(row, 8).Value = o.PlannedStartAt;
            sheet.Cell(row, 9).Value = o.PlannedEndAt;
            sheet.Cell(row, 10).Value = o.RequestorStatus;
            sheet.Cell(row, 11).Value = o.PlannerStatus;
            sheet.Cell(row, 12).Value = o.GnmStatus;
            sheet.Cell(row, 13).Value = o.GncStatus;
            sheet.Cell(row, 14).Value = o.Description;
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportPdfAsync(ReportFilterDto filter)
    {
        var result = await _outageService.ListAsync(ToOutageFilter(filter));
        var rows = result.Data ?? new List<OutageListItemDto>();

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Text("TNB ICOMS 2.0 — Outage Report").FontSize(16).Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.3f); // Outage Number
                        columns.RelativeColumn(0.9f); // Type
                        columns.RelativeColumn(1f);   // Zone
                        columns.RelativeColumn(1f);   // Station
                        columns.RelativeColumn(1.2f); // Equipment
                        columns.RelativeColumn(1.4f); // Planned Start
                        columns.RelativeColumn(1.4f); // Planned End
                        columns.RelativeColumn(1f);   // Requestor Status
                        columns.RelativeColumn(1f);   // GNM Status
                    });

                    table.Header(header =>
                    {
                        foreach (var text in new[] { "Outage No.", "Type", "Zone", "Station", "Equipment", "Planned Start", "Planned End", "Requestor", "GNM" })
                        {
                            header.Cell().Element(c => c.Padding(4).Background(Colors.Grey.Lighten2)).Text(text).Bold();
                        }
                    });

                    foreach (var o in rows)
                    {
                        table.Cell().Padding(4).Text(o.OutageNumber);
                        table.Cell().Padding(4).Text(o.OutageTypeCode);
                        table.Cell().Padding(4).Text(o.ZoneName ?? "-");
                        table.Cell().Padding(4).Text(o.StationName ?? "-");
                        table.Cell().Padding(4).Text(o.EquipmentName ?? "-");
                        table.Cell().Padding(4).Text(o.PlannedStartAt.ToString("g"));
                        table.Cell().Padding(4).Text(o.PlannedEndAt.ToString("g"));
                        table.Cell().Padding(4).Text(o.RequestorStatus);
                        table.Cell().Padding(4).Text(o.GnmStatus);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated ").FontSize(7);
                    x.Span(DateTime.UtcNow.ToString("u")).FontSize(7);
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<ApiResponse<List<SavedReportFilterDto>>> ListFavouritesAsync(int userId)
    {
        var favourites = await _dbContext.SavedReportFilters
            .Where(f => f.UserId == userId && f.ReportCode == "CustomReport")
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return ApiResponse<List<SavedReportFilterDto>>.Ok(favourites.Select(Map).ToList());
    }

    public async Task<ApiResponse<SavedReportFilterDto>> SaveFavouriteAsync(int userId, SaveReportFilterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FilterName))
        {
            return ApiResponse<SavedReportFilterDto>.Fail("Favourite name is required.");
        }

        var entity = new SavedReportFilter
        {
            UserId = userId,
            FilterName = request.FilterName.Trim(),
            ReportCode = "CustomReport",
            FilterJson = JsonSerializer.Serialize(request.Filter),
            IsFavorite = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SavedReportFilters.Add(entity);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<SavedReportFilterDto>.Ok(Map(entity));
    }

    public async Task<ApiResponse<object>> DeleteFavouriteAsync(int userId, int savedReportFilterId)
    {
        var entity = await _dbContext.SavedReportFilters
            .FirstOrDefaultAsync(f => f.SavedReportFilterId == savedReportFilterId && f.UserId == userId);

        if (entity == null)
        {
            return ApiResponse<object>.Fail("Favourite not found.");
        }

        _dbContext.SavedReportFilters.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static SavedReportFilterDto Map(SavedReportFilter entity) => new()
    {
        SavedReportFilterId = entity.SavedReportFilterId,
        FilterName = entity.FilterName,
        Filter = JsonSerializer.Deserialize<ReportFilterDto>(entity.FilterJson) ?? new ReportFilterDto(),
        CreatedAt = entity.CreatedAt
    };
}
