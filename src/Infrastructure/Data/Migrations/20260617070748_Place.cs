using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Place : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Place",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(155)", maxLength: 155, nullable: true),
                    Information = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(95)", maxLength: 95, nullable: false),
                    Mobile = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    Landline = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    Instagram = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Place", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Place_AspNetUsers_BroId",
                        column: x => x.BroId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Place_Address",
                table: "Place",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Place_BroId",
                table: "Place",
                column: "BroId");

            migrationBuilder.CreateIndex(
                name: "IX_Place_Email",
                table: "Place",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Place_Instagram",
                table: "Place",
                column: "Instagram");

            migrationBuilder.CreateIndex(
                name: "IX_Place_Name",
                table: "Place",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Place");
        }
    }
}
