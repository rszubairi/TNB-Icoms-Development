using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Files;
using TnbIcoms.Application.SingleLineDiagrams.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.SingleLineDiagrams;

public class SldService : ISldService
{
    private const string SubFolder = "sld";
    private static readonly string[] FlowTypes = { "New", "Existing", "Update" };
    private static readonly string[] SubstationTypes = { "AIS", "GIS", "Hybrid" };

    private readonly AppDbContext _dbContext;
    private readonly IFileStorageService _fileStorage;

    public SldService(AppDbContext dbContext, IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResponse<List<SldListItemDto>>> ListAsync(int? stationId, string? status)
    {
        var query = _dbContext.SingleLineDiagrams.Include(s => s.Station).Include(s => s.VoltageLevel).AsQueryable();
        if (stationId.HasValue) query = query.Where(s => s.StationId == stationId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(s => s.Status == status);

        var slds = await query.OrderByDescending(s => s.SubmittedAt).ToListAsync();
        var userNames = await GetUserNamesAsync();

        return ApiResponse<List<SldListItemDto>>.Ok(slds.Select(s => MapListItem(s, userNames)).ToList());
    }

    public async Task<ApiResponse<SldDetailDto>> GetByIdAsync(int id)
    {
        var sld = await LoadAsync(id);
        if (sld is null) return ApiResponse<SldDetailDto>.Fail("Single Line Diagram not found.");

        var userNames = await GetUserNamesAsync();
        return ApiResponse<SldDetailDto>.Ok(MapDetail(sld, userNames));
    }

    public async Task<ApiResponse<SldDetailDto>> CreateAsync(CreateSldRequestDto request, int currentUserId)
    {
        if (!FlowTypes.Contains(request.FlowType)) return ApiResponse<SldDetailDto>.Fail("Flow type must be New, Existing, or Update.");

        var station = await _dbContext.Stations.FirstOrDefaultAsync(s => s.StationId == request.StationId && s.IsActive);
        if (station is null) return ApiResponse<SldDetailDto>.Fail("Selected station does not exist.");

        var voltageLevel = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == request.VoltageLevelId && v.IsActive);
        if (voltageLevel is null) return ApiResponse<SldDetailDto>.Fail("Selected voltage level does not exist.");

        var sld = new SingleLineDiagram
        {
            StationId = request.StationId,
            VoltageLevelId = request.VoltageLevelId,
            FlowType = request.FlowType,
            Remark = request.Remark,
            Status = "PendingEngineerReview",
            SubmittedBy = currentUserId,
            SubmittedAt = DateTime.UtcNow
        };

        _dbContext.SingleLineDiagrams.Add(sld);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(sld.SingleLineDiagramId);
    }

    public async Task<ApiResponse<SldDetailDto>> UploadDrawingAsync(int id, Stream content, string originalFileName, int currentUserId)
    {
        var sld = await _dbContext.SingleLineDiagrams.FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
        if (sld is null) return ApiResponse<SldDetailDto>.Fail("Single Line Diagram not found.");
        if (sld.Status != "PendingEngineerReview") return ApiResponse<SldDetailDto>.Fail("The drawing can only be uploaded while pending Engineer review.");

        var storedFileName = await _fileStorage.SaveAsync(content, SubFolder, originalFileName);
        sld.StoredFileName = storedFileName;
        sld.OriginalFileName = originalFileName;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<SldDetailDto>> EngineerReviewAsync(int id, EngineerReviewRequestDto request, int currentUserId)
    {
        var sld = await _dbContext.SingleLineDiagrams.Include(s => s.VoltageLevel).FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
        if (sld is null) return ApiResponse<SldDetailDto>.Fail("Single Line Diagram not found.");
        if (sld.Status != "PendingEngineerReview") return ApiResponse<SldDetailDto>.Fail("This diagram is not awaiting Engineer review.");

        if (!request.Approve)
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason)) return ApiResponse<SldDetailDto>.Fail("A rejection reason is required.");
            sld.Status = "Rejected";
            sld.RejectionReason = request.RejectionReason;
            sld.EngineerReviewedBy = currentUserId;
            sld.EngineerReviewedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        if (string.IsNullOrWhiteSpace(request.Mnemonic) || string.IsNullOrWhiteSpace(request.SubstationType) || !SubstationTypes.Contains(request.SubstationType))
        {
            return ApiResponse<SldDetailDto>.Fail("Mnemonic and a valid Substation Type (AIS, GIS, Hybrid) are required to approve.");
        }
        if (string.IsNullOrWhiteSpace(sld.StoredFileName))
        {
            return ApiResponse<SldDetailDto>.Fail("A drawing must be uploaded before this diagram can move forward.");
        }

        var runningNumber = 1 + await _dbContext.SingleLineDiagrams.CountAsync(s =>
            s.SingleLineDiagramId != id && s.VoltageLevelId == sld.VoltageLevelId && s.SubstationType == request.SubstationType && s.RunningNumber != null);

        var voltageIndex = sld.VoltageLevel!.SortOrder + 1;
        var typeCode = request.SubstationType[0].ToString().ToUpperInvariant();

        sld.Mnemonic = request.Mnemonic.Trim().ToUpperInvariant();
        sld.SubstationType = request.SubstationType;
        sld.RunningNumber = runningNumber;
        sld.DiagramNumber = $"{voltageIndex}-{typeCode}-{sld.Mnemonic}-{runningNumber:D3}";
        sld.Status = "PendingSE";
        sld.EngineerReviewedBy = currentUserId;
        sld.EngineerReviewedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public Task<ApiResponse<SldDetailDto>> SeReviewAsync(int id, StageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingSE", "PendingDCE", request, currentUserId,
            onApprove: sld => { sld.SeApprovedBy = currentUserId; sld.SeApprovedAt = DateTime.UtcNow; });

    public Task<ApiResponse<SldDetailDto>> DceReviewAsync(int id, StageReviewRequestDto request, int currentUserId) =>
        AdvanceStageAsync(id, "PendingDCE", "PendingRequestorApproval", request, currentUserId,
            onApprove: sld => { sld.DceApprovedBy = currentUserId; sld.DceApprovedAt = DateTime.UtcNow; });

    public async Task<ApiResponse<SldDetailDto>> RequestorApproveAsync(int id, StageReviewRequestDto request, int currentUserId)
    {
        var sld = await _dbContext.SingleLineDiagrams.FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
        if (sld is null) return ApiResponse<SldDetailDto>.Fail("Single Line Diagram not found.");
        if (sld.Status != "PendingRequestorApproval") return ApiResponse<SldDetailDto>.Fail("This diagram is not awaiting Requestor approval.");

        if (!request.Approve)
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason)) return ApiResponse<SldDetailDto>.Fail("A reason is required to request adjustment.");
            sld.Status = "PendingEngineerReview";
            sld.RejectionReason = request.RejectionReason;
            await _dbContext.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        sld.Status = "Published";
        sld.RequestorApprovedBy = currentUserId;
        sld.RequestorApprovedAt = DateTime.UtcNow;
        sld.PublishedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<(Stream? Content, string? FileName)> OpenDrawingAsync(int id)
    {
        var sld = await _dbContext.SingleLineDiagrams.FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
        if (sld?.StoredFileName is null || sld.OriginalFileName is null) return (null, null);

        return (_fileStorage.OpenRead(SubFolder, sld.StoredFileName), sld.OriginalFileName);
    }

    /// <summary>
    /// URS §5.10: at S/E and DCE stages, "Request Adjustment" loops the diagram back to the
    /// GNM Engineer stage rather than terminating it, unlike the Engineer's own reject action.
    /// </summary>
    private async Task<ApiResponse<SldDetailDto>> AdvanceStageAsync(
        int id, string expectedStatus, string nextStatus, StageReviewRequestDto request, int currentUserId, Action<SingleLineDiagram> onApprove)
    {
        var sld = await _dbContext.SingleLineDiagrams.FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
        if (sld is null) return ApiResponse<SldDetailDto>.Fail("Single Line Diagram not found.");
        if (sld.Status != expectedStatus) return ApiResponse<SldDetailDto>.Fail($"This diagram is not awaiting this stage's review (current status: {sld.Status}).");

        if (!request.Approve)
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason)) return ApiResponse<SldDetailDto>.Fail("A reason is required to request adjustment.");
            sld.Status = "PendingEngineerReview";
            sld.RejectionReason = request.RejectionReason;
            await _dbContext.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        onApprove(sld);
        sld.Status = nextStatus;
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    private async Task<SingleLineDiagram?> LoadAsync(int id)
    {
        return await _dbContext.SingleLineDiagrams
            .Include(s => s.Station)
            .Include(s => s.VoltageLevel)
            .FirstOrDefaultAsync(s => s.SingleLineDiagramId == id);
    }

    private async Task<Dictionary<int, string>> GetUserNamesAsync()
    {
        return await _dbContext.AppUsers.ToDictionaryAsync(u => u.UserId, u => u.FullName);
    }

    private static SldListItemDto MapListItem(SingleLineDiagram sld, Dictionary<int, string> userNames)
    {
        return new SldListItemDto
        {
            SingleLineDiagramId = sld.SingleLineDiagramId,
            StationId = sld.StationId,
            StationName = sld.Station?.StationName,
            VoltageLevelName = sld.VoltageLevel?.LevelName,
            FlowType = sld.FlowType,
            Mnemonic = sld.Mnemonic,
            SubstationType = sld.SubstationType,
            DiagramNumber = sld.DiagramNumber,
            Status = sld.Status,
            HasDrawing = !string.IsNullOrEmpty(sld.StoredFileName),
            SubmittedByName = userNames.TryGetValue(sld.SubmittedBy, out var n) ? n : string.Empty,
            SubmittedAt = sld.SubmittedAt
        };
    }

    private static SldDetailDto MapDetail(SingleLineDiagram sld, Dictionary<int, string> userNames)
    {
        var listItem = MapListItem(sld, userNames);
        string? Name(int? id) => id.HasValue && userNames.TryGetValue(id.Value, out var n) ? n : null;

        return new SldDetailDto
        {
            SingleLineDiagramId = listItem.SingleLineDiagramId,
            StationId = listItem.StationId,
            StationName = listItem.StationName,
            VoltageLevelName = listItem.VoltageLevelName,
            FlowType = listItem.FlowType,
            Mnemonic = listItem.Mnemonic,
            SubstationType = listItem.SubstationType,
            DiagramNumber = listItem.DiagramNumber,
            Status = listItem.Status,
            HasDrawing = listItem.HasDrawing,
            SubmittedByName = listItem.SubmittedByName,
            SubmittedAt = listItem.SubmittedAt,
            VoltageLevelId = sld.VoltageLevelId,
            RejectionReason = sld.RejectionReason,
            Remark = sld.Remark,
            EngineerReviewedByName = Name(sld.EngineerReviewedBy),
            EngineerReviewedAt = sld.EngineerReviewedAt,
            SeApprovedByName = Name(sld.SeApprovedBy),
            SeApprovedAt = sld.SeApprovedAt,
            DceApprovedByName = Name(sld.DceApprovedBy),
            DceApprovedAt = sld.DceApprovedAt,
            RequestorApprovedByName = Name(sld.RequestorApprovedBy),
            RequestorApprovedAt = sld.RequestorApprovedAt,
            PublishedAt = sld.PublishedAt
        };
    }
}
