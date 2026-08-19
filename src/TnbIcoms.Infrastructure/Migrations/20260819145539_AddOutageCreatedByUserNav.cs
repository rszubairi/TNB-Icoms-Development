using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutageCreatedByUserNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Outages_CreatedBy",
                schema: "dbo",
                table: "Outages",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Outages_Users_CreatedBy",
                schema: "dbo",
                table: "Outages",
                column: "CreatedBy",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Outages_Users_CreatedBy",
                schema: "dbo",
                table: "Outages");

            migrationBuilder.DropIndex(
                name: "IX_Outages_CreatedBy",
                schema: "dbo",
                table: "Outages");
        }
    }
}
