using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VkOrdApiWrapper.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseScriptsAndFixRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RowVersion",
                table: "Users",
                type: "integer",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<int>(
                name: "RowVersion",
                table: "ApiCredentials",
                type: "integer",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.CreateTable(
                name: "DatabaseScripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScriptName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ScriptHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseScripts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseScripts_ScriptName",
                table: "DatabaseScripts",
                column: "ScriptName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseScripts");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Users",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "ApiCredentials",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldRowVersion: true);
        }
    }
}
