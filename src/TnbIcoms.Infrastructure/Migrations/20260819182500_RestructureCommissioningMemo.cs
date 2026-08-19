using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureCommissioningMemo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "SubmittedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "SwitchingProgram");

            migrationBuilder.RenameColumn(
                name: "ApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "SeApprovedBy");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "SeApprovedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CeGnmApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CeGnmApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommissioningResult",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataForm",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DceApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DceApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EngineerPicApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngineerPicApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FormG",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormH",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HgsoLetterForGenerationPmu",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IomEndorsed",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MemoType",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MeteringEmailChain",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MtepProtectionLetter",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResidentEngineerCertification",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ScadaEmailChain",
                schema: "dbo",
                table: "CommissioningMemos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CommissioningMemos_MemoNo",
                schema: "dbo",
                table: "CommissioningMemos",
                column: "MemoNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommissioningMemos_MemoNo",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "CeGnmApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "CeGnmApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "CommissioningResult",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "DataForm",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "DceApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "DceApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "EngineerPicApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "EngineerPicApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "FinalApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "FinalApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "FormG",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "FormH",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "HgsoLetterForGenerationPmu",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "IomEndorsed",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "MemoType",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "MeteringEmailChain",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "MtepProtectionLetter",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "ResidentEngineerCertification",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.DropColumn(
                name: "ScadaEmailChain",
                schema: "dbo",
                table: "CommissioningMemos");

            migrationBuilder.RenameColumn(
                name: "SwitchingProgram",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "SubmittedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "SeApprovedBy",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "ApprovedBy");

            migrationBuilder.RenameColumn(
                name: "SeApprovedAt",
                schema: "dbo",
                table: "CommissioningMemos",
                newName: "ApprovedAt");
        }
    }
}
