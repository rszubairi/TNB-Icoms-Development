using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureOutagePicAndAddOutageNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicContact",
                schema: "dbo",
                table: "OutagePics");

            migrationBuilder.DropColumn(
                name: "PicRole",
                schema: "dbo",
                table: "OutagePics");

            migrationBuilder.AddColumn<string>(
                name: "PicEmail",
                schema: "dbo",
                table: "OutagePics",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PicPhone",
                schema: "dbo",
                table: "OutagePics",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outages_OutageNumber",
                schema: "dbo",
                table: "Outages",
                column: "OutageNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Outages_OutageNumber",
                schema: "dbo",
                table: "Outages");

            migrationBuilder.DropColumn(
                name: "PicEmail",
                schema: "dbo",
                table: "OutagePics");

            migrationBuilder.DropColumn(
                name: "PicPhone",
                schema: "dbo",
                table: "OutagePics");

            migrationBuilder.AddColumn<string>(
                name: "PicContact",
                schema: "dbo",
                table: "OutagePics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PicRole",
                schema: "dbo",
                table: "OutagePics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
