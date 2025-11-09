using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class _20251109184801_AddErirSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VkOrdErirStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogicalAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    ErirStatus = table.Column<int>(type: "integer", nullable: false),
                    UpdatedByUserTs = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinalizedTs = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ErrorMessages = table.Column<List<string>>(type: "text[]", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdErirStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_EntityType",
                table: "VkOrdErirStatuses",
                column: "EntityType",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_ErirStatus",
                table: "VkOrdErirStatuses",
                column: "ErirStatus",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_LogicalAccountId",
                table: "VkOrdErirStatuses",
                column: "LogicalAccountId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_LogicalAccountId_ExternalId_EntityType",
                table: "VkOrdErirStatuses",
                columns: new[] { "LogicalAccountId", "ExternalId", "EntityType" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_PublicId",
                table: "VkOrdErirStatuses",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdErirStatuses_UpdatedByUserTs",
                table: "VkOrdErirStatuses",
                column: "UpdatedByUserTs",
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkOrdErirStatuses");
        }
    }
}
