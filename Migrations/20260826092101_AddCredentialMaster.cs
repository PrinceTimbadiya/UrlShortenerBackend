using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortenerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCredentialMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CredentialMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SecretKeyHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialMaster_UserMaster_UserId",
                        column: x => x.UserId,
                        principalTable: "UserMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CredentialMaster_ApiKey",
                table: "CredentialMaster",
                column: "ApiKey",
                unique: true,
                filter: "[ApiKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialMaster_UserId",
                table: "CredentialMaster",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredentialMaster");
        }
    }
}
