namespace TnbIcoms.Application.Roles;

/// <summary>
/// Canonical list of feature modules and the permission actions each supports.
/// Drives the permission matrix shown in the Role editor UI.
/// </summary>
public static class PermissionModules
{
    public static readonly string[] StandardActions = { "View", "Create", "Edit", "Delete", "Approve" };

    public static readonly IReadOnlyList<(string Code, string Label)> Modules = new List<(string, string)>
    {
        ("USERS", "Users"),
        ("ROLES", "Roles & Permissions"),
        ("ZONES", "Zones & Locations"),
        ("ORGANISATIONS", "Organisations & Stations"),
        ("EQUIPMENT", "Voltage & Equipment"),
        ("OUTAGES", "Outage Requests"),
        ("OUTAGE_APPROVAL", "Outage Approval / Docket"),
        ("AUTHORISATION", "Authorisation & Off-Points"),
        ("SLD", "Single Line Diagrams"),
        ("COMM_MEMO", "Commissioning Memos"),
        ("HANDOVER", "Shift Handover"),
        ("REPORTS", "Reports & Statistics")
    };
}
