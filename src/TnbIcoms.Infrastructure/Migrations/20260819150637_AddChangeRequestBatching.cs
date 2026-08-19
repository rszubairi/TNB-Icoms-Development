using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeRequestBatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                schema: "dbo",
                table: "ChangeRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                schema: "dbo",
                table: "ChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_BatchId",
                schema: "dbo",
                table: "ChangeRequests",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChangeRequests_BatchId",
                schema: "dbo",
                table: "ChangeRequests");

            migrationBuilder.DropColumn(
                name: "BatchId",
                schema: "dbo",
                table: "ChangeRequests");

            migrationBuilder.DropColumn(
                name: "ReviewComment",
                schema: "dbo",
                table: "ChangeRequests");
        }
    }
}
