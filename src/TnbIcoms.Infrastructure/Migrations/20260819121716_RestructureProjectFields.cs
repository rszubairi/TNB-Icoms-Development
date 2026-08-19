using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureProjectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectCode",
                schema: "config",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                schema: "config",
                table: "Projects",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ProjectSuffix",
                schema: "config",
                table: "Projects",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TpCode",
                schema: "config",
                table: "Projects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TpCode",
                schema: "config",
                table: "Projects",
                column: "TpCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_TpCode",
                schema: "config",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectSuffix",
                schema: "config",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TpCode",
                schema: "config",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                schema: "config",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.AddColumn<string>(
                name: "ProjectCode",
                schema: "config",
                table: "Projects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
