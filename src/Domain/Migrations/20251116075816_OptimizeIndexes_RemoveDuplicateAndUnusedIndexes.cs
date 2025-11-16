using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeIndexes_RemoveDuplicateAndUnusedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_CreatedAt",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_CreativeExternalId",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_DateEndActual",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_DateEndPlanned",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_DateStartActual",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_DateStartPlanned",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_ExpiresAt",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_LogicalAccountId",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_PadExternalId",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_Period",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_StatisticsType",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_SyncStatus",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdStatistics_UpdatedAt",
                table: "VkOrdStatistics");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdMedias_CreatedAt",
                table: "VkOrdMedias");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdMedias_ExpiresAt",
                table: "VkOrdMedias");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdMedias_LogicalAccountId",
                table: "VkOrdMedias");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdMedias_SyncStatus",
                table: "VkOrdMedias");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdMedias_UpdatedAt",
                table: "VkOrdMedias");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_ContractExternalId",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_CreatedAt",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_ExpiresAt",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_IsDraft",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_LogicalAccountId",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_SyncStatus",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdInvoices_UpdatedAt",
                table: "VkOrdInvoices");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreatives_CreatedAt",
                table: "VkOrdCreatives");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreatives_ExpiresAt",
                table: "VkOrdCreatives");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreatives_LogicalAccountId",
                table: "VkOrdCreatives");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreatives_SyncStatus",
                table: "VkOrdCreatives");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreatives_UpdatedAt",
                table: "VkOrdCreatives");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreativeMedia_CreatedAt",
                table: "VkOrdCreativeMedia");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreativeMedia_CreativeId",
                table: "VkOrdCreativeMedia");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreativeMedia_Order",
                table: "VkOrdCreativeMedia");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreativeContract_CreatedAt",
                table: "VkOrdCreativeContract");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCreativeContract_CreativeId",
                table: "VkOrdCreativeContract");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCounterparties_CreatedAt",
                table: "VkOrdCounterparties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCounterparties_ExpiresAt",
                table: "VkOrdCounterparties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCounterparties_LogicalAccountId",
                table: "VkOrdCounterparties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCounterparties_SyncStatus",
                table: "VkOrdCounterparties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdCounterparties_UpdatedAt",
                table: "VkOrdCounterparties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContracts_CreatedAt",
                table: "VkOrdContracts");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContracts_ExpiresAt",
                table: "VkOrdContracts");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContracts_LogicalAccountId",
                table: "VkOrdContracts");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContracts_SyncStatus",
                table: "VkOrdContracts");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContracts_UpdatedAt",
                table: "VkOrdContracts");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContractParties_ContractId",
                table: "VkOrdContractParties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContractParties_CreatedAt",
                table: "VkOrdContractParties");

            migrationBuilder.DropIndex(
                name: "IX_VkOrdContractParties_Role",
                table: "VkOrdContractParties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_CreatedAt",
                table: "VkOrdStatistics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_CreativeExternalId",
                table: "VkOrdStatistics",
                column: "CreativeExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_DateEndActual",
                table: "VkOrdStatistics",
                column: "DateEndActual");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_DateEndPlanned",
                table: "VkOrdStatistics",
                column: "DateEndPlanned");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_DateStartActual",
                table: "VkOrdStatistics",
                column: "DateStartActual");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_DateStartPlanned",
                table: "VkOrdStatistics",
                column: "DateStartPlanned");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_ExpiresAt",
                table: "VkOrdStatistics",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_LogicalAccountId",
                table: "VkOrdStatistics",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_PadExternalId",
                table: "VkOrdStatistics",
                column: "PadExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_Period",
                table: "VkOrdStatistics",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_StatisticsType",
                table: "VkOrdStatistics",
                column: "StatisticsType");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_SyncStatus",
                table: "VkOrdStatistics",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_UpdatedAt",
                table: "VkOrdStatistics",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_CreatedAt",
                table: "VkOrdMedias",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_ExpiresAt",
                table: "VkOrdMedias",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_LogicalAccountId",
                table: "VkOrdMedias",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_SyncStatus",
                table: "VkOrdMedias",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_UpdatedAt",
                table: "VkOrdMedias",
                column: "UpdatedAt");

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
                name: "IX_VkOrdInvoices_SyncStatus",
                table: "VkOrdInvoices",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdInvoices_UpdatedAt",
                table: "VkOrdInvoices",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_CreatedAt",
                table: "VkOrdCreatives",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_ExpiresAt",
                table: "VkOrdCreatives",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_LogicalAccountId",
                table: "VkOrdCreatives",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_SyncStatus",
                table: "VkOrdCreatives",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_UpdatedAt",
                table: "VkOrdCreatives",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_CreatedAt",
                table: "VkOrdCreativeMedia",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_CreativeId",
                table: "VkOrdCreativeMedia",
                column: "CreativeId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_Order",
                table: "VkOrdCreativeMedia",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeContract_CreatedAt",
                table: "VkOrdCreativeContract",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeContract_CreativeId",
                table: "VkOrdCreativeContract",
                column: "CreativeId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_CreatedAt",
                table: "VkOrdCounterparties",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_ExpiresAt",
                table: "VkOrdCounterparties",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_LogicalAccountId",
                table: "VkOrdCounterparties",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_SyncStatus",
                table: "VkOrdCounterparties",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_UpdatedAt",
                table: "VkOrdCounterparties",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_CreatedAt",
                table: "VkOrdContracts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_ExpiresAt",
                table: "VkOrdContracts",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_LogicalAccountId",
                table: "VkOrdContracts",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_SyncStatus",
                table: "VkOrdContracts",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_UpdatedAt",
                table: "VkOrdContracts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_ContractId",
                table: "VkOrdContractParties",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_CreatedAt",
                table: "VkOrdContractParties",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_Role",
                table: "VkOrdContractParties",
                column: "Role");
        }
    }
}
