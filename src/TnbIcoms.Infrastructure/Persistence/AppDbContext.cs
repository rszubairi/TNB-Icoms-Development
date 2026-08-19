using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TnbIcoms.Domain.Entities.Audit;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Domain.Entities.Handover;
using TnbIcoms.Infrastructure.Identity;
using AuthEntities = TnbIcoms.Domain.Entities.Auth;
using OutageEntities = TnbIcoms.Domain.Entities.Outage;

namespace TnbIcoms.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Auth schema
    public DbSet<AuthEntities.User> AppUsers => Set<AuthEntities.User>();
    public DbSet<AuthEntities.Role> AppRoles => Set<AuthEntities.Role>();
    public DbSet<AuthEntities.RolePermission> RolePermissions => Set<AuthEntities.RolePermission>();
    public DbSet<AuthEntities.UserGcuStation> UserGcuStations => Set<AuthEntities.UserGcuStation>();
    public DbSet<AuthEntities.RoleTransferRequest> RoleTransferRequests => Set<AuthEntities.RoleTransferRequest>();

    // Config schema
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<ZoneLocation> ZoneLocations => Set<ZoneLocation>();
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<VoltageLevel> VoltageLevels => Set<VoltageLevel>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<ConflictingLine> ConflictingLines => Set<ConflictingLine>();
    public DbSet<DropdownValue> DropdownValues => Set<DropdownValue>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<OutageTypeRule> OutageTypeRules => Set<OutageTypeRule>();
    public DbSet<AuthorisationPersonnel> AuthorisationPersonnel => Set<AuthorisationPersonnel>();

    // dbo (Outage) schema
    public DbSet<OutageEntities.Outage> Outages => Set<OutageEntities.Outage>();
    public DbSet<OutageEntities.OutageAdditionalEquipment> OutageAdditionalEquipment => Set<OutageEntities.OutageAdditionalEquipment>();
    public DbSet<OutageEntities.OutagePic> OutagePics => Set<OutageEntities.OutagePic>();
    public DbSet<OutageEntities.OutageNotifyEmail> OutageNotifyEmails => Set<OutageEntities.OutageNotifyEmail>();
    public DbSet<OutageEntities.ChangeRequest> ChangeRequests => Set<OutageEntities.ChangeRequest>();
    public DbSet<OutageEntities.OutageOffPoint> OutageOffPoints => Set<OutageEntities.OutageOffPoint>();
    public DbSet<OutageEntities.GcuAcknowledgement> GcuAcknowledgements => Set<OutageEntities.GcuAcknowledgement>();
    public DbSet<OutageEntities.Authorisation> Authorisations => Set<OutageEntities.Authorisation>();
    public DbSet<OutageEntities.SingleLineDiagram> SingleLineDiagrams => Set<OutageEntities.SingleLineDiagram>();
    public DbSet<OutageEntities.CommissioningMemo> CommissioningMemos => Set<OutageEntities.CommissioningMemo>();

    // Handover schema
    public DbSet<HandoverShift> HandoverShifts => Set<HandoverShift>();
    public DbSet<HandoverEntry> HandoverEntries => Set<HandoverEntry>();

    // Audit schema
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SavedReportFilter> SavedReportFilters => Set<SavedReportFilter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
