using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTransferRequestZoneFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ToRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FromRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FromZoneId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToZoneId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromZoneId",
                schema: "auth",
                table: "RoleTransferRequests");

            migrationBuilder.DropColumn(
                name: "ToZoneId",
                schema: "auth",
                table: "RoleTransferRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ToRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromRoleId",
                schema: "auth",
                table: "RoleTransferRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
