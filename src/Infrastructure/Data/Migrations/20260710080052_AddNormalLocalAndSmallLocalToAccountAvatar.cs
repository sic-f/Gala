using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalLocalAndSmallLocalToAccountAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalLocal",
                table: "AccountAvatar",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmallLocal",
                table: "AccountAvatar",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalLocal",
                table: "AccountAvatar");

            migrationBuilder.DropColumn(
                name: "SmallLocal",
                table: "AccountAvatar");
        }
    }
}
