using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Domain.Entities.Outage;

public class Outage
{
    public int OutageId { get; set; }
    public string OutageNumber { get; set; } = string.Empty; // e.g. ICOMS-2026-00001
    public char OutageCode { get; set; } // P, U, E, F
    public string OutageTypeCode { get; set; } = string.Empty; // Planned, Unplanned, Emergency, Forced
    public string OutageClass { get; set; } = string.Empty; // Maintenance, Project
    public string WorkTypeCode { get; set; } = string.Empty; // Dead, Live
    public int ZoneId { get; set; }
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public int PrimaryEquipmentId { get; set; }
    public string? LineFilterType { get; set; }
    public int JobTypeId { get; set; }
    public int? ProjectId { get; set; }
    public int? SequenceId { get; set; }
    public int? RestorationId { get; set; }
    public DateTime PlannedStartAt { get; set; }
    public DateTime PlannedEndAt { get; set; }
    public DateTime? ActualStartAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public DateTime? ExtendedEndAt { get; set; }
    public bool HasPtw { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ContingencyPlanUrl { get; set; }

    // Status tracking across four perspectives
    public string RequestorStatus { get; set; } = "Draft"; // Draft, Pending, Confirmed, Outage Closed - Unconfirmed
    public string? PlannerStatus { get; set; } // Agreed, Disagreed
    public string GnmStatus { get; set; } = "Pending"; // Pending, Under-Study, KIV, Approved, Disapproved, Outage Closed - KIV Timeout
    public string? GncStatus { get; set; } // Taken-Active, Taken-Completed, Outage Closed, Outage Closed - Not Taken, Outage Closed - Cancelled by GNC

    public DateTime? DsoAgreedAt { get; set; }
    public bool DsoAgreedBySystem { get; set; }

    // Study fields by TOMS/GNM
    public string? Justification { get; set; }
    public string? Highlights { get; set; }
    public string? Remark { get; set; }
    public string? UnderStudyNotes { get; set; }

    public int? NotTakenReasonId { get; set; }
    public int? ApprovedById { get; set; }
    public bool IsGcuNotified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public Zone? Zone { get; set; }
    public Station? Station { get; set; }
    public VoltageLevel? VoltageLevel { get; set; }
    public Equipment? PrimaryEquipment { get; set; }
    public Project? Project { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<OutageAdditionalEquipment> AdditionalEquipment { get; set; } = new List<OutageAdditionalEquipment>();
    public ICollection<OutagePic> Pics { get; set; } = new List<OutagePic>();
    public ICollection<OutageNotifyEmail> NotifyEmails { get; set; } = new List<OutageNotifyEmail>();
    public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
    public ICollection<OutageOffPoint> OffPoints { get; set; } = new List<OutageOffPoint>();
    public ICollection<GcuAcknowledgement> GcuAcknowledgements { get; set; } = new List<GcuAcknowledgement>();
    public ICollection<Authorisation> Authorisations { get; set; } = new List<Authorisation>();
    public ICollection<SingleLineDiagram> SingleLineDiagrams { get; set; } = new List<SingleLineDiagram>();
    public ICollection<CommissioningMemo> CommissioningMemos { get; set; } = new List<CommissioningMemo>();
}
