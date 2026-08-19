using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationZoneAndUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganisationCode",
                schema: "config",
                table: "Organisations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZoneId",
                schema: "config",
                table: "Organisations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationAbbr",
                schema: "config",
                table: "Stations",
                column: "StationAbbr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationName",
                schema: "config",
                table: "Stations",
                column: "StationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_OrganisationCode",
                schema: "config",
                table: "Organisations",
                column: "OrganisationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_OrganisationName",
                schema: "config",
                table: "Organisations",
                column: "OrganisationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_ZoneId",
                schema: "config",
                table: "Organisations",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_Zones_ZoneId",
                schema: "config",
                table: "Organisations",
                column: "ZoneId",
                principalSchema: "config",
                principalTable: "Zones",
                principalColumn: "ZoneId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_Zones_ZoneId",
                schema: "config",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_StationAbbr",
                schema: "config",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_StationName",
                schema: "config",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_OrganisationCode",
                schema: "config",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_OrganisationName",
                schema: "config",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_ZoneId",
                schema: "config",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                schema: "config",
                table: "Organisations");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationCode",
                schema: "config",
                table: "Organisations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
