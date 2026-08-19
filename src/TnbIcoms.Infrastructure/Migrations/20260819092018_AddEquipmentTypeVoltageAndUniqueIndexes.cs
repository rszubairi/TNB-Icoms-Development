using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentTypeVoltageAndUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoltageLevelId",
                schema: "config",
                table: "EquipmentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VoltageLevels_LevelName",
                schema: "config",
                table: "VoltageLevels",
                column: "LevelName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_VoltageLevelId_TypeName",
                schema: "config",
                table: "EquipmentTypes",
                columns: new[] { "VoltageLevelId", "TypeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentCode",
                schema: "config",
                table: "Equipment",
                column: "EquipmentCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_VoltageLevels_VoltageLevelId",
                schema: "config",
                table: "EquipmentTypes",
                column: "VoltageLevelId",
                principalSchema: "config",
                principalTable: "VoltageLevels",
                principalColumn: "VoltageLevelId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_VoltageLevels_VoltageLevelId",
                schema: "config",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_VoltageLevels_LevelName",
                schema: "config",
                table: "VoltageLevels");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTypes_VoltageLevelId_TypeName",
                schema: "config",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_EquipmentCode",
                schema: "config",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "VoltageLevelId",
                schema: "config",
                table: "EquipmentTypes");
        }
    }
}
