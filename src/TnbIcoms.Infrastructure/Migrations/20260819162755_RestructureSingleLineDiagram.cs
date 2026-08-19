using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureSingleLineDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SingleLineDiagrams_Outages_OutageId",
                schema: "dbo",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                schema: "dbo",
                table: "SingleLineDiagrams");

            migrationBuilder.RenameTable(
                name: "SingleLineDiagrams",
                schema: "dbo",
                newName: "SingleLineDiagrams",
                newSchema: "config");

            migrationBuilder.RenameColumn(
                name: "UploadedBy",
                schema: "config",
                table: "SingleLineDiagrams",
                newName: "VoltageLevelId");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "RevisionNo",
                schema: "config",
                table: "SingleLineDiagrams",
                newName: "SubmittedBy");

            migrationBuilder.RenameColumn(
                name: "OutageId",
                schema: "config",
                table: "SingleLineDiagrams",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_SingleLineDiagrams_OutageId",
                schema: "config",
                table: "SingleLineDiagrams",
                newName: "IX_SingleLineDiagrams_StationId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DceApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DceApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagramNumber",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EngineerReviewedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngineerReviewedBy",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlowType",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mnemonic",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestorApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestorApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RunningNumber",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SeApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubstationType",
                schema: "config",
                table: "SingleLineDiagrams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SingleLineDiagrams_VoltageLevelId",
                schema: "config",
                table: "SingleLineDiagrams",
                column: "VoltageLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SingleLineDiagrams_Stations_StationId",
                schema: "config",
                table: "SingleLineDiagrams",
                column: "StationId",
                principalSchema: "config",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SingleLineDiagrams_VoltageLevels_VoltageLevelId",
                schema: "config",
                table: "SingleLineDiagrams",
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
                name: "FK_SingleLineDiagrams_Stations_StationId",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropForeignKey(
                name: "FK_SingleLineDiagrams_VoltageLevels_VoltageLevelId",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropIndex(
                name: "IX_SingleLineDiagrams_VoltageLevelId",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "DceApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "DceApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "DiagramNumber",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "EngineerReviewedAt",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "EngineerReviewedBy",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "FlowType",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "Mnemonic",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "RequestorApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "RequestorApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "RunningNumber",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "SeApprovedAt",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "SeApprovedBy",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "StoredFileName",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.DropColumn(
                name: "SubstationType",
                schema: "config",
                table: "SingleLineDiagrams");

            migrationBuilder.RenameTable(
                name: "SingleLineDiagrams",
                schema: "config",
                newName: "SingleLineDiagrams",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "VoltageLevelId",
                schema: "dbo",
                table: "SingleLineDiagrams",
                newName: "UploadedBy");

            migrationBuilder.RenameColumn(
                name: "SubmittedBy",
                schema: "dbo",
                table: "SingleLineDiagrams",
                newName: "RevisionNo");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                schema: "dbo",
                table: "SingleLineDiagrams",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "StationId",
                schema: "dbo",
                table: "SingleLineDiagrams",
                newName: "OutageId");

            migrationBuilder.RenameIndex(
                name: "IX_SingleLineDiagrams_StationId",
                schema: "dbo",
                table: "SingleLineDiagrams",
                newName: "IX_SingleLineDiagrams_OutageId");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                schema: "dbo",
                table: "SingleLineDiagrams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SingleLineDiagrams_Outages_OutageId",
                schema: "dbo",
                table: "SingleLineDiagrams",
                column: "OutageId",
                principalSchema: "dbo",
                principalTable: "Outages",
                principalColumn: "OutageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
