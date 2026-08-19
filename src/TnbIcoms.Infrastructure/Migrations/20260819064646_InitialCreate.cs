using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "handover");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TnbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "audit",
                columns: table => new
                {
                    AuditLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "DropdownValues",
                schema: "config",
                columns: table => new
                {
                    DropdownValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValueCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValueLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropdownValues", x => x.DropdownValueId);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                schema: "config",
                columns: table => new
                {
                    EquipmentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.EquipmentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                schema: "config",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganisationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsGcu = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.OrganisationId);
                });

            migrationBuilder.CreateTable(
                name: "OutageTypeRules",
                schema: "config",
                columns: table => new
                {
                    OutageTypeRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageTypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinLeadDays = table.Column<int>(type: "int", nullable: true),
                    MaxLeadDays = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutageTypeRules", x => x.OutageTypeRuleId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SavedReportFilters",
                schema: "audit",
                columns: table => new
                {
                    SavedReportFilterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FilterName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReportCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilterJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedReportFilters", x => x.SavedReportFilterId);
                });

            migrationBuilder.CreateTable(
                name: "VoltageLevels",
                schema: "config",
                columns: table => new
                {
                    VoltageLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageLevels", x => x.VoltageLevelId);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                schema: "config",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ZoneAbbr = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ZoneId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "auth",
                columns: table => new
                {
                    RolePermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ModuleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorisationPersonnel",
                schema: "config",
                columns: table => new
                {
                    AuthorisationPersonnelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorisationPersonnel", x => x.AuthorisationPersonnelId);
                    table.ForeignKey(
                        name: "FK_AuthorisationPersonnel_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HandoverShifts",
                schema: "handover",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    ControlManagerId = table.Column<int>(type: "int", nullable: true),
                    SwitchEngineer1Id = table.Column<int>(type: "int", nullable: true),
                    SwitchEngineer2Id = table.Column<int>(type: "int", nullable: true),
                    DespatcherId = table.Column<int>(type: "int", nullable: true),
                    ControlAssistantId = table.Column<int>(type: "int", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    PassedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PassedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverShifts", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_HandoverShifts_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "config",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZoneId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                schema: "config",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationAbbr = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    SldFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.StationId);
                    table.ForeignKey(
                        name: "FK_Stations_Organisations_OrgId",
                        column: x => x.OrgId,
                        principalSchema: "config",
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stations_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TnbId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AuthType = table.Column<byte>(type: "tinyint", nullable: false),
                    AspNetUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: true),
                    GcuTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalSchema: "config",
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZoneLocations",
                schema: "config",
                columns: table => new
                {
                    ZoneLocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneLocations", x => x.ZoneLocationId);
                    table.ForeignKey(
                        name: "FK_ZoneLocations_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandoverEntries",
                schema: "handover",
                columns: table => new
                {
                    HandoverEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedOutageId = table.Column<int>(type: "int", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverEntries", x => x.HandoverEntryId);
                    table.ForeignKey(
                        name: "FK_HandoverEntries_HandoverShifts_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "handover",
                        principalTable: "HandoverShifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "config",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EquipmentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "int", nullable: false),
                    VoltageLevelId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    MvaRatingId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<byte>(type: "tinyint", nullable: false),
                    IsOffPoint = table.Column<bool>(type: "bit", nullable: false),
                    OffPointRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineFilterType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipment_EquipmentTypes_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalSchema: "config",
                        principalTable: "EquipmentTypes",
                        principalColumn: "EquipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipment_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "config",
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipment_VoltageLevels_VoltageLevelId",
                        column: x => x.VoltageLevelId,
                        principalSchema: "config",
                        principalTable: "VoltageLevels",
                        principalColumn: "VoltageLevelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipment_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleTransferRequests",
                schema: "auth",
                columns: table => new
                {
                    RoleTransferRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FromRoleId = table.Column<int>(type: "int", nullable: false),
                    ToRoleId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedBy = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTransferRequests", x => x.RoleTransferRequestId);
                    table.ForeignKey(
                        name: "FK_RoleTransferRequests_Roles_FromRoleId",
                        column: x => x.FromRoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleTransferRequests_Roles_ToRoleId",
                        column: x => x.ToRoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleTransferRequests_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGcuStations",
                schema: "auth",
                columns: table => new
                {
                    UserGcuStationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGcuStations", x => x.UserGcuStationId);
                    table.ForeignKey(
                        name: "FK_UserGcuStations_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "config",
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGcuStations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConflictingLines",
                schema: "config",
                columns: table => new
                {
                    ConflictingLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    ConflictingEquipmentId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictingLines", x => x.ConflictingLineId);
                    table.ForeignKey(
                        name: "FK_ConflictingLines_Equipment_ConflictingEquipmentId",
                        column: x => x.ConflictingEquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictingLines_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Outages",
                schema: "dbo",
                columns: table => new
                {
                    OutageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutageCode = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    OutageTypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OutageClass = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WorkTypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    VoltageLevelId = table.Column<int>(type: "int", nullable: false),
                    PrimaryEquipmentId = table.Column<int>(type: "int", nullable: false),
                    LineFilterType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTypeId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    SequenceId = table.Column<int>(type: "int", nullable: true),
                    RestorationId = table.Column<int>(type: "int", nullable: true),
                    PlannedStartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtendedEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasPtw = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContingencyPlanUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlannerStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GnmStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GncStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DsoAgreedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DsoAgreedBySystem = table.Column<bool>(type: "bit", nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnderStudyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotTakenReasonId = table.Column<int>(type: "int", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    IsGcuNotified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outages", x => x.OutageId);
                    table.ForeignKey(
                        name: "FK_Outages_Equipment_PrimaryEquipmentId",
                        column: x => x.PrimaryEquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "config",
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outages_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "config",
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outages_VoltageLevels_VoltageLevelId",
                        column: x => x.VoltageLevelId,
                        principalSchema: "config",
                        principalTable: "VoltageLevels",
                        principalColumn: "VoltageLevelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outages_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Authorisations",
                schema: "dbo",
                columns: table => new
                {
                    AuthorisationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    AuthorisationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    TakenActiveAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TakenCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorisations", x => x.AuthorisationId);
                    table.ForeignKey(
                        name: "FK_Authorisations_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                schema: "dbo",
                columns: table => new
                {
                    ChangeRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedBy = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.ChangeRequestId);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissioningMemos",
                schema: "dbo",
                columns: table => new
                {
                    CommissioningMemoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    MemoNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissioningMemos", x => x.CommissioningMemoId);
                    table.ForeignKey(
                        name: "FK_CommissioningMemos_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GcuAcknowledgements",
                schema: "dbo",
                columns: table => new
                {
                    GcuAcknowledgementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    GcuUserId = table.Column<int>(type: "int", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAutoAgreed = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GcuAcknowledgements", x => x.GcuAcknowledgementId);
                    table.ForeignKey(
                        name: "FK_GcuAcknowledgements_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutageAdditionalEquipment",
                schema: "dbo",
                columns: table => new
                {
                    OutageAdditionalEquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutageAdditionalEquipment", x => x.OutageAdditionalEquipmentId);
                    table.ForeignKey(
                        name: "FK_OutageAdditionalEquipment_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutageAdditionalEquipment_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutageNotifyEmails",
                schema: "dbo",
                columns: table => new
                {
                    OutageNotifyEmailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false),
                    NotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutageNotifyEmails", x => x.OutageNotifyEmailId);
                    table.ForeignKey(
                        name: "FK_OutageNotifyEmails_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutageOffPoints",
                schema: "dbo",
                columns: table => new
                {
                    OutageOffPointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutageOffPoints", x => x.OutageOffPointId);
                    table.ForeignKey(
                        name: "FK_OutageOffPoints_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutageOffPoints_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutagePics",
                schema: "dbo",
                columns: table => new
                {
                    OutagePicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    PicName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PicContact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PicRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutagePics", x => x.OutagePicId);
                    table.ForeignKey(
                        name: "FK_OutagePics_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleLineDiagrams",
                schema: "dbo",
                columns: table => new
                {
                    SingleLineDiagramId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutageId = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleLineDiagrams", x => x.SingleLineDiagramId);
                    table.ForeignKey(
                        name: "FK_SingleLineDiagrams_Outages_OutageId",
                        column: x => x.OutageId,
                        principalSchema: "dbo",
                        principalTable: "Outages",
                        principalColumn: "OutageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorisationPersonnel_ZoneId",
                schema: "config",
                table: "AuthorisationPersonnel",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorisations_OutageId",
                schema: "dbo",
                table: "Authorisations",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_OutageId",
                schema: "dbo",
                table: "ChangeRequests",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissioningMemos_OutageId",
                schema: "dbo",
                table: "CommissioningMemos",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictingLines_ConflictingEquipmentId",
                schema: "config",
                table: "ConflictingLines",
                column: "ConflictingEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictingLines_EquipmentId",
                schema: "config",
                table: "ConflictingLines",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentTypeId",
                schema: "config",
                table: "Equipment",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_StationId",
                schema: "config",
                table: "Equipment",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_VoltageLevelId",
                schema: "config",
                table: "Equipment",
                column: "VoltageLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_ZoneId",
                schema: "config",
                table: "Equipment",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GcuAcknowledgements_OutageId",
                schema: "dbo",
                table: "GcuAcknowledgements",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverEntries_ShiftId",
                schema: "handover",
                table: "HandoverEntries",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverShifts_ZoneId",
                schema: "handover",
                table: "HandoverShifts",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_OutageAdditionalEquipment_EquipmentId",
                schema: "dbo",
                table: "OutageAdditionalEquipment",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutageAdditionalEquipment_OutageId",
                schema: "dbo",
                table: "OutageAdditionalEquipment",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutageNotifyEmails_OutageId",
                schema: "dbo",
                table: "OutageNotifyEmails",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutageOffPoints_EquipmentId",
                schema: "dbo",
                table: "OutageOffPoints",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutageOffPoints_OutageId",
                schema: "dbo",
                table: "OutageOffPoints",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutagePics_OutageId",
                schema: "dbo",
                table: "OutagePics",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_Outages_PrimaryEquipmentId",
                schema: "dbo",
                table: "Outages",
                column: "PrimaryEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Outages_ProjectId",
                schema: "dbo",
                table: "Outages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Outages_StationId",
                schema: "dbo",
                table: "Outages",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Outages_VoltageLevelId",
                schema: "dbo",
                table: "Outages",
                column: "VoltageLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Outages_ZoneId",
                schema: "dbo",
                table: "Outages",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ZoneId",
                schema: "config",
                table: "Projects",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "auth",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTransferRequests_FromRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                column: "FromRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTransferRequests_ToRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                column: "ToRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTransferRequests_UserId",
                schema: "auth",
                table: "RoleTransferRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleLineDiagrams_OutageId",
                schema: "dbo",
                table: "SingleLineDiagrams",
                column: "OutageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_OrgId",
                schema: "config",
                table: "Stations",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_ZoneId",
                schema: "config",
                table: "Stations",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGcuStations_StationId",
                schema: "auth",
                table: "UserGcuStations",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGcuStations_UserId",
                schema: "auth",
                table: "UserGcuStations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganisationId",
                schema: "auth",
                table: "Users",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "auth",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ZoneId",
                schema: "auth",
                table: "Users",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneLocations_ZoneId",
                schema: "config",
                table: "ZoneLocations",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "AuthorisationPersonnel",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Authorisations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChangeRequests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CommissioningMemos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ConflictingLines",
                schema: "config");

            migrationBuilder.DropTable(
                name: "DropdownValues",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GcuAcknowledgements",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HandoverEntries",
                schema: "handover");

            migrationBuilder.DropTable(
                name: "OutageAdditionalEquipment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OutageNotifyEmails",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OutageOffPoints",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OutagePics",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OutageTypeRules",
                schema: "config");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "RoleTransferRequests",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "SavedReportFilters",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "SingleLineDiagrams",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserGcuStations",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ZoneLocations",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HandoverShifts",
                schema: "handover");

            migrationBuilder.DropTable(
                name: "Outages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "EquipmentTypes",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Stations",
                schema: "config");

            migrationBuilder.DropTable(
                name: "VoltageLevels",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Organisations",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Zones",
                schema: "config");
        }
    }
}
