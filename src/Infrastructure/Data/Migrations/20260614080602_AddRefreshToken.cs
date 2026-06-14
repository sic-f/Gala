using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Revoked = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    BroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_BroId",
                        column: x => x.BroId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_BroId",
                table: "RefreshToken",
                column: "BroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Expires_Revoked",
                table: "RefreshToken",
                columns: new[] { "Expires", "Revoked" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_ReplacedByToken",
                table: "RefreshToken",
                column: "ReplacedByToken",
                unique: true,
                filter: "[ReplacedByToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");
        }
    }
}
