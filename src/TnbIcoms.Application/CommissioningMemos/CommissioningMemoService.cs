using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.CommissioningMemos.Dtos;
using TnbIcoms.Domain.Entities.Outage;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.CommissioningMemos;

public class CommissioningMemoService : ICommissioningMemoService
{
    private static readonly string[] MemoTypes = { "Commissioning", "EmergencyCommissioning", "Decommissioning" };
    private static readonly string[] CommissioningResults = { "InProgress", "OnSoak", "CommSuccessful", "CommNotSuccessful" };
    private static readonly Dictionary<string, string> TypeCodes = new()
    {
        ["Commissioning"] = "COMM",
        ["EmergencyCommissioning"] = "ECOM",
        ["Decommissioning"] = "DECOM"
    };

    private readonly AppDbContext _dbContext;

    public CommissioningMemoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<CommissioningMemoListItemDto>>> ListAsync(int? outageId, string? status)
    {
        var query = _dbContext.CommissioningMemos.Include(m => m.Outage).AsQueryable();
        if (outageId.HasValue) query = query.Where(m => m.OutageId == outageId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(m => m.Status == status);

        var memos = await query.OrderByDescending(m => m.SubmittedAt).ToListAsync();
        var userNames = await GetUserNamesAsync();

        return ApiResponse<List<CommissioningMemoListItemDto>>.Ok(memos.Select(m => MapListItem(m, userNames)).ToList());
    }

    public async Task<ApiResponse<CommissioningMemoDetailDto>> GetByIdAsync(int id)
    {
        var memo = await LoadAsync(id);
        if (memo is null) return ApiResponse<CommissioningMemoDetailDto>.Fail("Commissioning Memo not found.");

        var userNames = await GetUserNamesAsync();
        return ApiResponse<CommissioningMemoDetailDto>.Ok(MapDetail(memo, userNames));
    }

    public async Task<ApiResponse<CommissioningMemoDetailDto>> CreateAsync(CreateCommissioningMemoRequestDto request, int currentUserId)
    {
        if (!MemoTypes.Contains(request.MemoType)) return ApiResponse<CommissioningMemoDetailDto>.Fail("Invalid memo type.");
        if (string.IsNullOrWhiteSpace(request.SwitchingProgram)) return ApiResponse<CommissioningMemoDetailDto>.Fail("Switching Program is required.");

        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == request.OutageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<CommissioningMemoDetailDto>.Fail("Selected outage does not exist.");

        var isEmergency = request.MemoType == "EmergencyCommissioning";
        if (!isEmergency && string.IsNullOrWhiteSpace(request.DataForm))
        {
            return ApiResponse<CommissioningMemoDetailDto>.Fail("Data Form is required for this memo type.");
        }

        var memoNo = await GenerateMemoNoAsync(request.MemoType);

        var memo = new CommissioningMemo
        {
            OutageId = request.OutageId,
            MemoNo = memoNo,
            MemoType = request.MemoType,
            SwitchingProgram = request.SwitchingProgram.Trim(),
            DataForm = isEmergency ? null : request.DataForm,
            IomEndorsed = !isEmergency && request.IomEndorsed,
            MtepProtectionLetter = !isEmergency && request.MtepProtectionLetter,
            ResidentEngineerCertification = !isEmergency && request.ResidentEngineerCertification,
            FormG = !isEmergency && request.FormG,
            FormH = !isEmergency && request.FormH,
            MeteringEmailChain = !isEmergency && request.MeteringEmailChain,
            ScadaEmailChain = !isEmergency && request.ScadaEmailChain,
            HgsoLetterForGenerationPmu = !isEmergency && request.HgsoLetterForGenerationPmu,
            Status = "PendingEngineerPic",
            SubmittedBy = currentUserId,
            SubmittedAt = DateTime.UtcNow
        };

        _dbContext.CommissioningMemos.Add(memo);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(memo.CommissioningMemoId);
    }

    public Task<ApiResponse<CommissioningMemoDetailDto>> EngineerPicReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingEngineerPic", "PendingSE", request, currentUserId,
            onApprove: m => { m.EngineerPicApprovedBy = currentUserId; m.EngineerPicApprovedAt = DateTime.UtcNow; });

    public Task<ApiResponse<CommissioningMemoDetailDto>> SeReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingSE", "PendingDCE", request, currentUserId,
            onApprove: m => { m.SeApprovedBy = currentUserId; m.SeApprovedAt = DateTime.UtcNow; });

    public Task<ApiResponse<CommissioningMemoDetailDto>> DceReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingDCE", "PendingCeGnm", request, currentUserId,
            onApprove: m => { m.DceApprovedBy = currentUserId; m.DceApprovedAt = DateTime.UtcNow; });

    public Task<ApiResponse<CommissioningMemoDetailDto>> CeGnmReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingCeGnm", "PendingFinalSignOff", request, currentUserId,
            onApprove: m => { m.CeGnmApprovedBy = currentUserId; m.CeGnmApprovedAt = DateTime.UtcNow; });

    public Task<ApiResponse<CommissioningMemoDetailDto>> FinalSignOffAsync(int id, MemoStageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingFinalSignOff", "Approved", request, currentUserId,
            onApprove: m => { m.FinalApprovedBy = currentUserId; m.FinalApprovedAt = DateTime.UtcNow; });

    public async Task<ApiResponse<CommissioningMemoDetailDto>> SetCommissioningResultAsync(int id, SetCommissioningResultRequestDto request, int currentUserId)
    {
        if (!CommissioningResults.Contains(request.CommissioningResult))
        {
            return ApiResponse<CommissioningMemoDetailDto>.Fail("Invalid commissioning result.");
        }

        var memo = await _dbContext.CommissioningMemos.FirstOrDefaultAsync(m => m.CommissioningMemoId == id);
        if (memo is null) return ApiResponse<CommissioningMemoDetailDto>.Fail("Commissioning Memo not found.");
        if (memo.Status != "Approved") return ApiResponse<CommissioningMemoDetailDto>.Fail("The commissioning result can only be recorded once the memo is fully approved.");

        memo.CommissioningResult = request.CommissioningResult;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<byte[]> GenerateCoverPagePdfAsync(int id)
    {
        var memo = await LoadAsync(id);
        if (memo is null) return Array.Empty<byte>();

        var userNames = await GetUserNamesAsync();
        var dto = MapDetail(memo, userNames);

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("TNB ICOMS 2.0 — Commissioning Memo").FontSize(18).Bold();
                    col.Item().Text(dto.MemoNo).FontSize(13).SemiBold();
                });

                page.Content().Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text($"Type: {dto.MemoType}");
                    col.Item().Text($"Outage: {dto.OutageNumber}");
                    col.Item().Text($"Status: {dto.Status}");
                    if (dto.CommissioningResult is not null) col.Item().Text($"Commissioning Result: {dto.CommissioningResult}");
                    col.Item().PaddingTop(10).Text("Switching Program").Bold();
                    col.Item().Text(dto.SwitchingProgram);
                    if (dto.DataForm is not null)
                    {
                        col.Item().PaddingTop(10).Text("Data Form").Bold();
                        col.Item().Text(dto.DataForm);
                    }
                    if (dto.MemoType != "EmergencyCommissioning")
                    {
                        col.Item().PaddingTop(10).Text("Commissioning Requirement Documents").Bold();
                        col.Item().Text(ChecklistLine("IOM Endorsed", dto.IomEndorsed));
                        col.Item().Text(ChecklistLine("MTEP Protection Letter", dto.MtepProtectionLetter));
                        col.Item().Text(ChecklistLine("Resident Engineer Certification", dto.ResidentEngineerCertification));
                        col.Item().Text(ChecklistLine("Form G", dto.FormG));
                        col.Item().Text(ChecklistLine("Form H", dto.FormH));
                        col.Item().Text(ChecklistLine("Metering Email Chain", dto.MeteringEmailChain));
                        col.Item().Text(ChecklistLine("SCADA Email Chain", dto.ScadaEmailChain));
                        col.Item().Text(ChecklistLine("HGSO Letter (Generation PMU)", dto.HgsoLetterForGenerationPmu));
                    }

                    col.Item().PaddingTop(14).Text("Approval Chain").Bold();
                    col.Item().Text(ApprovalLine("Engineer PIC", dto.EngineerPicApprovedByName, dto.EngineerPicApprovedAt));
                    col.Item().Text(ApprovalLine("S/E", dto.SeApprovedByName, dto.SeApprovedAt));
                    col.Item().Text(ApprovalLine("DCE", dto.DceApprovedByName, dto.DceApprovedAt));
                    col.Item().Text(ApprovalLine("CE GNM", dto.CeGnmApprovedByName, dto.CeGnmApprovedAt));
                    col.Item().Text(ApprovalLine("Final Sign-off", dto.FinalApprovedByName, dto.FinalApprovedAt));
                });

                page.Footer().AlignCenter().Text(x => x.Span($"Generated {DateTime.UtcNow:u}").FontSize(7));
            });
        });

        return document.GeneratePdf();
    }

    private static string ChecklistLine(string label, bool value) => $"{(value ? "☑" : "☐")} {label}";
    private static string ApprovalLine(string stage, string? name, DateTime? at) =>
        name is null ? $"{stage}: pending" : $"{stage}: {name} — {at:g}";

    /// <summary>
    /// URS §5.11: rejection at any stage sends the memo back for revision. This simplifies
    /// the "return to rejecting stage" behavior to always route back to the first (Engineer
    /// PIC) stage, mirroring the SLD module's adjustment loop-back for consistency.
    /// </summary>
    private async Task<ApiResponse<CommissioningMemoDetailDto>> AdvanceStageAsync(
        int id, string expectedStatus, string nextStatus, MemoStageReviewRequestDto request, int currentUserId, Action<CommissioningMemo> onApprove)
    {
        var memo = await _dbContext.CommissioningMemos.FirstOrDefaultAsync(m => m.CommissioningMemoId == id);
        if (memo is null) return ApiResponse<CommissioningMemoDetailDto>.Fail("Commissioning Memo not found.");
        if (memo.Status != expectedStatus) return ApiResponse<CommissioningMemoDetailDto>.Fail($"This memo is not awaiting this stage's review (current status: {memo.Status}).");

        if (!request.Approve)
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason)) return ApiResponse<CommissioningMemoDetailDto>.Fail("A rejection reason is required.");
            memo.Status = "PendingEngineerPic";
            memo.RejectionReason = request.RejectionReason;
            await _dbContext.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        onApprove(memo);
        memo.Status = nextStatus;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    private async Task<string> GenerateMemoNoAsync(string memoType)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"{TypeCodes[memoType]}-{year}-";
        var count = await _dbContext.CommissioningMemos.CountAsync(m => m.MemoNo.StartsWith(prefix));
        return $"{prefix}{(count + 1):D5}";
    }

    private async Task<CommissioningMemo?> LoadAsync(int id)
    {
        return await _dbContext.CommissioningMemos.Include(m => m.Outage).FirstOrDefaultAsync(m => m.CommissioningMemoId == id);
    }

    private async Task<Dictionary<int, string>> GetUserNamesAsync()
    {
        return await _dbContext.AppUsers.ToDictionaryAsync(u => u.UserId, u => u.FullName);
    }

    private static CommissioningMemoListItemDto MapListItem(CommissioningMemo m, Dictionary<int, string> userNames)
    {
        return new CommissioningMemoListItemDto
        {
            CommissioningMemoId = m.CommissioningMemoId,
            OutageId = m.OutageId,
            OutageNumber = m.Outage?.OutageNumber,
            MemoNo = m.MemoNo,
            MemoType = m.MemoType,
            Status = m.Status,
            CommissioningResult = m.CommissioningResult,
            SubmittedByName = userNames.TryGetValue(m.SubmittedBy, out var n) ? n : string.Empty,
            SubmittedAt = m.SubmittedAt
        };
    }

    private static CommissioningMemoDetailDto MapDetail(CommissioningMemo m, Dictionary<int, string> userNames)
    {
        var listItem = MapListItem(m, userNames);
        string? Name(int? id) => id.HasValue && userNames.TryGetValue(id.Value, out var n) ? n : null;

        return new CommissioningMemoDetailDto
        {
            CommissioningMemoId = listItem.CommissioningMemoId,
            OutageId = listItem.OutageId,
            OutageNumber = listItem.OutageNumber,
            MemoNo = listItem.MemoNo,
            MemoType = listItem.MemoType,
            Status = listItem.Status,
            CommissioningResult = listItem.CommissioningResult,
            SubmittedByName = listItem.SubmittedByName,
            SubmittedAt = listItem.SubmittedAt,
            SwitchingProgram = m.SwitchingProgram,
            DataForm = m.DataForm,
            IomEndorsed = m.IomEndorsed,
            MtepProtectionLetter = m.MtepProtectionLetter,
            ResidentEngineerCertification = m.ResidentEngineerCertification,
            FormG = m.FormG,
            FormH = m.FormH,
            MeteringEmailChain = m.MeteringEmailChain,
            ScadaEmailChain = m.ScadaEmailChain,
            HgsoLetterForGenerationPmu = m.HgsoLetterForGenerationPmu,
            RejectionReason = m.RejectionReason,
            EngineerPicApprovedByName = Name(m.EngineerPicApprovedBy),
            EngineerPicApprovedAt = m.EngineerPicApprovedAt,
            SeApprovedByName = Name(m.SeApprovedBy),
            SeApprovedAt = m.SeApprovedAt,
            DceApprovedByName = Name(m.DceApprovedBy),
            DceApprovedAt = m.DceApprovedAt,
            CeGnmApprovedByName = Name(m.CeGnmApprovedBy),
            CeGnmApprovedAt = m.CeGnmApprovedAt,
            FinalApprovedByName = Name(m.FinalApprovedBy),
            FinalApprovedAt = m.FinalApprovedAt
        };
    }
}
