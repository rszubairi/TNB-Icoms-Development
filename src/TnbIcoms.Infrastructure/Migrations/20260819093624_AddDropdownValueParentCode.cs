using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDropdownValueParentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                schema: "config",
                table: "DropdownValues",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DropdownValues_CategoryCode_ValueCode",
                schema: "config",
                table: "DropdownValues",
                columns: new[] { "CategoryCode", "ValueCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DropdownValues_CategoryCode_ValueCode",
                schema: "config",
                table: "DropdownValues");

            migrationBuilder.DropColumn(
                name: "ParentCode",
                schema: "config",
                table: "DropdownValues");
        }
    }
}
