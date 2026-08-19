namespace TnbIcoms.Application.DropdownValues;

/// <summary>
/// Canonical dropdown categories managed by GNM ADMIN, per URS Module 1 §5.2.6.
/// </summary>
public static class DropdownCategories
{
    public static readonly IReadOnlyList<(string Code, string Label, bool HasParent)> Categories = new List<(string, string, bool)>
    {
        ("JobType", "Job Type", true),
        ("WorkType", "Work Type", false),
        ("Sequence", "Sequence", false),
        ("Restoration", "Restoration", false),
        ("NotTakenReason", "Not-Taken Reasoning", false),
        ("GcuType", "GCU Type", false),
        ("MvaRating", "MVA Rating", false)
    };

    public static readonly string[] OutageTypeParents = { "Planned", "Unplanned", "Emergency", "Forced" };
}
