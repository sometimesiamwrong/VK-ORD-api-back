using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddVkOrdInvoicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_VkOrdContracts_LogicalAccountId_ExternalId",
                table: "VkOrdContracts",
                columns: new[] { "LogicalAccountId", "ExternalId" });

            migrationBuilder.CreateTable(
                name: "VkOrdInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    ContractExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LogicalAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdInvoices_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdInvoices_VkOrdContracts_LogicalAccountId_ContractExter~",
                        columns: x => new { x.LogicalAccountId, x.ContractExternalId },
                        principalTable: "VkOrdContracts",
                        principalColumns: new[] { "LogicalAccountId", "ExternalId" },
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_ContractExternalId",
                table: "VkOrdInvoices",
                column: "ContractExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_CreatedAt",
                table: "VkOrdInvoices",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_ExpiresAt",
                table: "VkOrdInvoices",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_IsDraft",
                table: "VkOrdInvoices",
                column: "IsDraft");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_LogicalAccountId",
                table: "VkOrdInvoices",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_LogicalAccountId_ContractExternalId",
                table: "VkOrdInvoices",
                columns: new[] { "LogicalAccountId", "ContractExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_LogicalAccountId_ExternalId",
                table: "VkOrdInvoices",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_SyncStatus",
                table: "VkOrdInvoices",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_UpdatedAt",
                table: "VkOrdInvoices",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkOrdInvoices");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_VkOrdContracts_LogicalAccountId_ExternalId",
                table: "VkOrdContracts");
        }
    }
}
