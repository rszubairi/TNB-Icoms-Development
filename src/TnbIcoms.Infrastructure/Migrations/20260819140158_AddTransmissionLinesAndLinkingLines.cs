using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransmissionLinesAndLinkingLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransmissionLineId",
                schema: "config",
                table: "Equipment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LinkingLines",
                schema: "config",
                columns: table => new
                {
                    LinkingLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    LinkedEquipmentId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkingLines", x => x.LinkingLineId);
                    table.ForeignKey(
                        name: "FK_LinkingLines_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinkingLines_Equipment_LinkedEquipmentId",
                        column: x => x.LinkedEquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionLines",
                schema: "config",
                columns: table => new
                {
                    TransmissionLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoltageLevelId = table.Column<int>(type: "int", nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "int", nullable: false),
                    NamingInteger = table.Column<int>(type: "int", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionLines", x => x.TransmissionLineId);
                    table.ForeignKey(
                        name: "FK_TransmissionLines_EquipmentTypes_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalSchema: "config",
                        principalTable: "EquipmentTypes",
                        principalColumn: "EquipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionLines_VoltageLevels_VoltageLevelId",
                        column: x => x.VoltageLevelId,
                        principalSchema: "config",
                        principalTable: "VoltageLevels",
                        principalColumn: "VoltageLevelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionLineOwnerZones",
                schema: "config",
                columns: table => new
                {
                    TransmissionLineOwnerZoneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransmissionLineId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionLineOwnerZones", x => x.TransmissionLineOwnerZoneId);
                    table.ForeignKey(
                        name: "FK_TransmissionLineOwnerZones_TransmissionLines_TransmissionLineId",
                        column: x => x.TransmissionLineId,
                        principalSchema: "config",
                        principalTable: "TransmissionLines",
                        principalColumn: "TransmissionLineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransmissionLineOwnerZones_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "config",
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionLineStations",
                schema: "config",
                columns: table => new
                {
                    TransmissionLineStationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransmissionLineId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false),
                    GeneratedEquipmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionLineStations", x => x.TransmissionLineStationId);
                    table.ForeignKey(
                        name: "FK_TransmissionLineStations_Equipment_GeneratedEquipmentId",
                        column: x => x.GeneratedEquipmentId,
                        principalSchema: "config",
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionLineStations_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "config",
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionLineStations_TransmissionLines_TransmissionLineId",
                        column: x => x.TransmissionLineId,
                        principalSchema: "config",
                        principalTable: "TransmissionLines",
                        principalColumn: "TransmissionLineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_TransmissionLineId",
                schema: "config",
                table: "Equipment",
                column: "TransmissionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkingLines_EquipmentId",
                schema: "config",
                table: "LinkingLines",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkingLines_LinkedEquipmentId",
                schema: "config",
                table: "LinkingLines",
                column: "LinkedEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLineOwnerZones_TransmissionLineId_ZoneId",
                schema: "config",
                table: "TransmissionLineOwnerZones",
                columns: new[] { "TransmissionLineId", "ZoneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLineOwnerZones_ZoneId",
                schema: "config",
                table: "TransmissionLineOwnerZones",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLines_EquipmentTypeId",
                schema: "config",
                table: "TransmissionLines",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLines_VoltageLevelId",
                schema: "config",
                table: "TransmissionLines",
                column: "VoltageLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLineStations_GeneratedEquipmentId",
                schema: "config",
                table: "TransmissionLineStations",
                column: "GeneratedEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLineStations_StationId",
                schema: "config",
                table: "TransmissionLineStations",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionLineStations_TransmissionLineId",
                schema: "config",
                table: "TransmissionLineStations",
                column: "TransmissionLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_TransmissionLines_TransmissionLineId",
                schema: "config",
                table: "Equipment",
                column: "TransmissionLineId",
                principalSchema: "config",
                principalTable: "TransmissionLines",
                principalColumn: "TransmissionLineId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_TransmissionLines_TransmissionLineId",
                schema: "config",
                table: "Equipment");

            migrationBuilder.DropTable(
                name: "LinkingLines",
                schema: "config");

            migrationBuilder.DropTable(
                name: "TransmissionLineOwnerZones",
                schema: "config");

            migrationBuilder.DropTable(
                name: "TransmissionLineStations",
                schema: "config");

            migrationBuilder.DropTable(
                name: "TransmissionLines",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_TransmissionLineId",
                schema: "config",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "TransmissionLineId",
                schema: "config",
                table: "Equipment");
        }
    }
}
