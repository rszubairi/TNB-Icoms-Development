using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnbIcoms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMnemonicDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MnemonicDocuments",
                schema: "config",
                columns: table => new
                {
                    MnemonicDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MnemonicDocuments", x => x.MnemonicDocumentId);
                    table.ForeignKey(
                        name: "FK_MnemonicDocuments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MnemonicDocuments_UploadedBy",
                schema: "config",
                table: "MnemonicDocuments",
                column: "UploadedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MnemonicDocuments",
                schema: "config");
        }
    }
}
