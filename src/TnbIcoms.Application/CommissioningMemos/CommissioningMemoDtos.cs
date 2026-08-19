namespace TnbIcoms.Application.CommissioningMemos.Dtos;

public class CommissioningMemoListItemDto
{
    public int CommissioningMemoId { get; set; }
    public int OutageId { get; set; }
    public string? OutageNumber { get; set; }
    public string MemoNo { get; set; } = string.Empty;
    public string MemoType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CommissioningResult { get; set; }
    public string SubmittedByName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

public class CommissioningMemoDetailDto : CommissioningMemoListItemDto
{
    public string SwitchingProgram { get; set; } = string.Empty;
    public string? DataForm { get; set; }
    public bool IomEndorsed { get; set; }
    public bool MtepProtectionLetter { get; set; }
    public bool ResidentEngineerCertification { get; set; }
    public bool FormG { get; set; }
    public bool FormH { get; set; }
    public bool MeteringEmailChain { get; set; }
    public bool ScadaEmailChain { get; set; }
    public bool HgsoLetterForGenerationPmu { get; set; }
    public string? RejectionReason { get; set; }

    public string? EngineerPicApprovedByName { get; set; }
    public DateTime? EngineerPicApprovedAt { get; set; }
    public string? SeApprovedByName { get; set; }
    public DateTime? SeApprovedAt { get; set; }
    public string? DceApprovedByName { get; set; }
    public DateTime? DceApprovedAt { get; set; }
    public string? CeGnmApprovedByName { get; set; }
    public DateTime? CeGnmApprovedAt { get; set; }
    public string? FinalApprovedByName { get; set; }
    public DateTime? FinalApprovedAt { get; set; }
}

public class CreateCommissioningMemoRequestDto
{
    public int OutageId { get; set; }
    public string MemoType { get; set; } = string.Empty; // Commissioning, EmergencyCommissioning, Decommissioning
    public string SwitchingProgram { get; set; } = string.Empty;
    public string? DataForm { get; set; }
    public bool IomEndorsed { get; set; }
    public bool MtepProtectionLetter { get; set; }
    public bool ResidentEngineerCertification { get; set; }
    public bool FormG { get; set; }
    public bool FormH { get; set; }
    public bool MeteringEmailChain { get; set; }
    public bool ScadaEmailChain { get; set; }
    public bool HgsoLetterForGenerationPmu { get; set; }
}

public class MemoStageReviewRequestDto
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}

public class SetCommissioningResultRequestDto
{
    public string CommissioningResult { get; set; } = string.Empty; // InProgress, OnSoak, CommSuccessful, CommNotSuccessful
}
