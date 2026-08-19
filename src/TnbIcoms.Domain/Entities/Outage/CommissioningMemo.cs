namespace TnbIcoms.Domain.Entities.Outage;

/// <summary>
/// URS Module 3 §5.11: three sub-workflows (Commissioning, Emergency Commissioning,
/// Decommissioning) sharing one entity. Emergency Commissioning only fills the Switching
/// Program part; the other two also fill the Data Form and Requirement Documents checklist.
/// Sequential role-based approval chain: Engineer PIC -> S/E -> DCE -> CE GNM -> final CE
/// GNC/DCE sign-off -> Approved, after which GNC records the actual commissioning result.
/// </summary>
public class CommissioningMemo
{
    public int CommissioningMemoId { get; set; }
    public int OutageId { get; set; }
    public string MemoNo { get; set; } = string.Empty;
    public string MemoType { get; set; } = string.Empty; // Commissioning, EmergencyCommissioning, Decommissioning

    public string SwitchingProgram { get; set; } = string.Empty;
    public string? DataForm { get; set; } // not used for EmergencyCommissioning

    // Commissioning Requirement Documents checklist — not used for EmergencyCommissioning
    public bool IomEndorsed { get; set; }
    public bool MtepProtectionLetter { get; set; }
    public bool ResidentEngineerCertification { get; set; }
    public bool FormG { get; set; }
    public bool FormH { get; set; }
    public bool MeteringEmailChain { get; set; }
    public bool ScadaEmailChain { get; set; }
    public bool HgsoLetterForGenerationPmu { get; set; }

    public string Status { get; set; } = "PendingEngineerPic";
    // PendingEngineerPic, PendingSE, PendingDCE, PendingCeGnm, PendingFinalSignOff, Approved, Rejected
    public string? RejectionReason { get; set; }
    public string? CommissioningResult { get; set; } // InProgress, OnSoak, CommSuccessful, CommNotSuccessful — set by GNC once Approved

    public int SubmittedBy { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public int? EngineerPicApprovedBy { get; set; }
    public DateTime? EngineerPicApprovedAt { get; set; }
    public int? SeApprovedBy { get; set; }
    public DateTime? SeApprovedAt { get; set; }
    public int? DceApprovedBy { get; set; }
    public DateTime? DceApprovedAt { get; set; }
    public int? CeGnmApprovedBy { get; set; }
    public DateTime? CeGnmApprovedAt { get; set; }
    public int? FinalApprovedBy { get; set; }
    public DateTime? FinalApprovedAt { get; set; }

    public Outage? Outage { get; set; }
}
