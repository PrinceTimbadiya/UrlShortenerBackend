using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortenerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LongUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlMaster_UserMaster_UserId",
                        column: x => x.UserId,
                        principalTable: "UserMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrlMaster_ShortCode",
                table: "UrlMaster",
                column: "ShortCode",
                unique: true,
                filter: "[ShortCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UrlMaster_UserId",
                table: "UrlMaster",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlMaster");
        }
    }
}
