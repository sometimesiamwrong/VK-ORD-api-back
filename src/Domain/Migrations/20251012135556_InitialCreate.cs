using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseScripts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkLogicalAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkLogicalAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiCredentials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiEnvironment = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    LogicalAccountId = table.Column<long>(type: "bigint", nullable: false),
                    TokenEncrypted = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiCredentials_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdContracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    VkOrdContractId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_VkOrdContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdContracts_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdContracts_VkOrdContracts_VkOrdContractId",
                        column: x => x.VkOrdContractId,
                        principalTable: "VkOrdContracts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCounterparties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    LogicalAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCounterparties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdCounterparties_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCreatives",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_VkOrdCreatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdCreatives_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdMedias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_VkOrdMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdMedias_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdContractParties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CounterpartyId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdContractParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdContractParties_VkOrdContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "VkOrdContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdContractParties_VkOrdCounterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "VkOrdCounterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCreativeContract",
                columns: table => new
                {
                    CreativeId = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCreativeContract", x => new { x.CreativeId, x.ContractId });
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeContract_VkOrdContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "VkOrdContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeContract_VkOrdCreatives_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreativeExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreativeId = table.Column<long>(type: "bigint", nullable: true),
                    PadExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ShowsCount = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceShowsCount = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<string>(type: "jsonb", nullable: true),
                    AmountPerEvent = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    PayType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateStartPlanned = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateEndPlanned = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateStartActual = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateEndActual = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Period = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    StatisticsType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_VkOrdStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdStatistics_VkLogicalAccounts_LogicalAccountId",
                        column: x => x.LogicalAccountId,
                        principalTable: "VkLogicalAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdStatistics_VkOrdCreatives_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCreativeMedia",
                columns: table => new
                {
                    CreativeId = table.Column<long>(type: "bigint", nullable: false),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCreativeMedia", x => new { x.CreativeId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeMedia_VkOrdCreatives_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeMedia_VkOrdMedias_MediaId",
                        column: x => x.MediaId,
                        principalTable: "VkOrdMedias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiCredentials_LogicalAccountId",
                table: "ApiCredentials",
                column: "LogicalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiCredentials_PublicId",
                table: "ApiCredentials",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiCredentials_UserId",
                table: "ApiCredentials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseScripts_ScriptName",
                table: "DatabaseScripts",
                column: "ScriptName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PublicId",
                table: "RefreshTokens",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PublicId",
                table: "Users",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkLogicalAccounts_PublicId",
                table: "VkLogicalAccounts",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_ContractId",
                table: "VkOrdContractParties",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_ContractId_CounterpartyId_Role",
                table: "VkOrdContractParties",
                columns: new[] { "ContractId", "CounterpartyId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_CounterpartyId",
                table: "VkOrdContractParties",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_CreatedAt",
                table: "VkOrdContractParties",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParties_Role",
                table: "VkOrdContractParties",
                column: "Role");

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
                name: "IX_VkOrdContracts_LogicalAccountId_ExternalId",
                table: "VkOrdContracts",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_SyncStatus",
                table: "VkOrdContracts",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_UpdatedAt",
                table: "VkOrdContracts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContracts_VkOrdContractId",
                table: "VkOrdContracts",
                column: "VkOrdContractId");

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
                name: "IX_VkOrdCounterparties_LogicalAccountId_ExternalId",
                table: "VkOrdCounterparties",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_SyncStatus",
                table: "VkOrdCounterparties",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterparties_UpdatedAt",
                table: "VkOrdCounterparties",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeContract_ContractId",
                table: "VkOrdCreativeContract",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeContract_CreatedAt",
                table: "VkOrdCreativeContract",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeContract_CreativeId",
                table: "VkOrdCreativeContract",
                column: "CreativeId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_CreatedAt",
                table: "VkOrdCreativeMedia",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_CreativeId",
                table: "VkOrdCreativeMedia",
                column: "CreativeId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_MediaId",
                table: "VkOrdCreativeMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeMedia_Order",
                table: "VkOrdCreativeMedia",
                column: "Order");

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
                name: "IX_VkOrdCreatives_LogicalAccountId_ExternalId",
                table: "VkOrdCreatives",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_SyncStatus",
                table: "VkOrdCreatives",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreatives_UpdatedAt",
                table: "VkOrdCreatives",
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
                name: "IX_VkOrdMedias_LogicalAccountId_ExternalId",
                table: "VkOrdMedias",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_SyncStatus",
                table: "VkOrdMedias",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMedias_UpdatedAt",
                table: "VkOrdMedias",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_CreatedAt",
                table: "VkOrdStatistics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_CreativeExternalId",
                table: "VkOrdStatistics",
                column: "CreativeExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatistics_CreativeId",
                table: "VkOrdStatistics",
                column: "CreativeId");

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
                name: "IX_VkOrdStatistics_LogicalAccountId_ExternalId",
                table: "VkOrdStatistics",
                columns: new[] { "LogicalAccountId", "ExternalId" },
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCredentials");

            migrationBuilder.DropTable(
                name: "DatabaseScripts");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "VkOrdContractParties");

            migrationBuilder.DropTable(
                name: "VkOrdCreativeContract");

            migrationBuilder.DropTable(
                name: "VkOrdCreativeMedia");

            migrationBuilder.DropTable(
                name: "VkOrdStatistics");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VkOrdCounterparties");

            migrationBuilder.DropTable(
                name: "VkOrdContracts");

            migrationBuilder.DropTable(
                name: "VkOrdMedias");

            migrationBuilder.DropTable(
                name: "VkOrdCreatives");

            migrationBuilder.DropTable(
                name: "VkLogicalAccounts");
        }
    }
}
