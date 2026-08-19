using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentShortName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "config",
                table: "Equipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "config",
                table: "Equipment");
        }
    }
}
