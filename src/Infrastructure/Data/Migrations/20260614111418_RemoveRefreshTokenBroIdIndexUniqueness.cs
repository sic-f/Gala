using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRefreshTokenBroIdIndexUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_BroId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_BroId",
                table: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_BroId",
                table: "RefreshToken",
                column: "BroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_BroId",
                table: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_BroId",
                table: "RefreshToken",
                column: "BroId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_BroId",
                table: "RefreshToken",
                column: "BroId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
