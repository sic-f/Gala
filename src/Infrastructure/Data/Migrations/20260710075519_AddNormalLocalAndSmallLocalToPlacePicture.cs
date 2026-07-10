using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalLocalAndSmallLocalToPlacePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalLocal",
                table: "PlacePicture",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmallLocal",
                table: "PlacePicture",
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
                table: "PlacePicture");

            migrationBuilder.DropColumn(
                name: "SmallLocal",
                table: "PlacePicture");
        }
    }
}
