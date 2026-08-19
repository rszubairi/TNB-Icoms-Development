using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureOutageTypeRulesAndAddScheduleWindows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.RenameColumn(
                name: "MinLeadDays",
                schema: "config",
                table: "OutageTypeRules",
                newName: "MoreThanYears");

            migrationBuilder.RenameColumn(
                name: "MaxLeadDays",
                schema: "config",
                table: "OutageTypeRules",
                newName: "MoreThanMonths");

            migrationBuilder.AddColumn<string>(
                name: "AppliesTo",
                schema: "config",
                table: "OutageTypeRules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LessThanDays",
                schema: "config",
                table: "OutageTypeRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LessThanMonths",
                schema: "config",
                table: "OutageTypeRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LessThanYears",
                schema: "config",
                table: "OutageTypeRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoreThanDays",
                schema: "config",
                table: "OutageTypeRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkTypeCode",
                schema: "config",
                table: "OutageTypeRules",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OutageScheduleWindows",
                schema: "config",
                columns: table => new
                {
                    OutageScheduleWindowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OutageTypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutageScheduleWindows", x => x.OutageScheduleWindowId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutageScheduleWindows_WorkTypeCode_OutageTypeCode_Month",
                schema: "config",
                table: "OutageScheduleWindows",
                columns: new[] { "WorkTypeCode", "OutageTypeCode", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutageScheduleWindows",
                schema: "config");

            migrationBuilder.DropColumn(
                name: "AppliesTo",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.DropColumn(
                name: "LessThanDays",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.DropColumn(
                name: "LessThanMonths",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.DropColumn(
                name: "LessThanYears",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.DropColumn(
                name: "MoreThanDays",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.DropColumn(
                name: "WorkTypeCode",
                schema: "config",
                table: "OutageTypeRules");

            migrationBuilder.RenameColumn(
                name: "MoreThanYears",
                schema: "config",
                table: "OutageTypeRules",
                newName: "MinLeadDays");

            migrationBuilder.RenameColumn(
                name: "MoreThanMonths",
                schema: "config",
                table: "OutageTypeRules",
                newName: "MaxLeadDays");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "config",
                table: "OutageTypeRules",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
