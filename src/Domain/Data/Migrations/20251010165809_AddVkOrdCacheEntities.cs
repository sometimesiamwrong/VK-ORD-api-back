using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domain.Data
{
    /// <inheritdoc />
    public partial class AddVkOrdCacheEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VkOrdContractCache",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContractorExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubjectType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Serial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Flags = table.Column<string>(type: "jsonb", nullable: true),
                    ParentContractExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ParentContractId = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    HasAdditionalContracts = table.Column<bool>(type: "boolean", nullable: false),
                    Cid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LockedFields = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    ApiCredentialId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CachedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdContractCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdContractCache_ApiCredentials_ApiCredentialId",
                        column: x => x.ApiCredentialId,
                        principalTable: "ApiCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdContractCache_VkOrdContractCache_ParentContractId",
                        column: x => x.ParentContractId,
                        principalTable: "VkOrdContractCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCounterpartyCache",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RsUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Roles = table.Column<string>(type: "jsonb", nullable: true),
                    JuridicalDetails = table.Column<string>(type: "jsonb", nullable: true),
                    LastUpdatedInVkOrd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    ApiCredentialId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CachedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCounterpartyCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdCounterpartyCache_ApiCredentials_ApiCredentialId",
                        column: x => x.ApiCredentialId,
                        principalTable: "ApiCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCreativeCache",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Erid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PersonExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Kktus = table.Column<string>(type: "jsonb", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Brand = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PayType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Form = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Targeting = table.Column<string>(type: "jsonb", nullable: true),
                    TargetUrls = table.Column<string>(type: "jsonb", nullable: true),
                    Texts = table.Column<string>(type: "jsonb", nullable: true),
                    MediaExternalIds = table.Column<string>(type: "jsonb", nullable: true),
                    Flags = table.Column<string>(type: "jsonb", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    ApiCredentialId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CachedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCreativeCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeCache_ApiCredentials_ApiCredentialId",
                        column: x => x.ApiCredentialId,
                        principalTable: "ApiCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdMediaCache",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Filename = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MediaType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DownloadUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UploadStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SyncStatus = table.Column<int>(type: "integer", nullable: false),
                    ApiCredentialId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CachedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdMediaCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdMediaCache_ApiCredentials_ApiCredentialId",
                        column: x => x.ApiCredentialId,
                        principalTable: "ApiCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdContractParty",
                columns: table => new
                {
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CounterpartyId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    VkOrdCounterpartyCacheId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdContractParty", x => new { x.ContractId, x.CounterpartyId, x.Role });
                    table.ForeignKey(
                        name: "FK_VkOrdContractParty_VkOrdContractCache_ContractId",
                        column: x => x.ContractId,
                        principalTable: "VkOrdContractCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdContractParty_VkOrdCounterpartyCache_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "VkOrdCounterpartyCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdContractParty_VkOrdCounterpartyCache_VkOrdCounterparty~",
                        column: x => x.VkOrdCounterpartyCacheId,
                        principalTable: "VkOrdCounterpartyCache",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VkOrdCounterpartyRelation",
                columns: table => new
                {
                    FromCounterpartyId = table.Column<long>(type: "bigint", nullable: false),
                    ToCounterpartyId = table.Column<long>(type: "bigint", nullable: false),
                    RelationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdCounterpartyRelation", x => new { x.FromCounterpartyId, x.ToCounterpartyId, x.RelationType });
                    table.ForeignKey(
                        name: "FK_VkOrdCounterpartyRelation_VkOrdCounterpartyCache_FromCounte~",
                        column: x => x.FromCounterpartyId,
                        principalTable: "VkOrdCounterpartyCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdCounterpartyRelation_VkOrdCounterpartyCache_ToCounterp~",
                        column: x => x.ToCounterpartyId,
                        principalTable: "VkOrdCounterpartyCache",
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
                        name: "FK_VkOrdCreativeContract_VkOrdContractCache_ContractId",
                        column: x => x.ContractId,
                        principalTable: "VkOrdContractCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeContract_VkOrdCreativeCache_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreativeCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkOrdStatisticsCache",
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
                    ApiCredentialId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CachedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkOrdStatisticsCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkOrdStatisticsCache_ApiCredentials_ApiCredentialId",
                        column: x => x.ApiCredentialId,
                        principalTable: "ApiCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdStatisticsCache_VkOrdCreativeCache_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreativeCache",
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
                        name: "FK_VkOrdCreativeMedia_VkOrdCreativeCache_CreativeId",
                        column: x => x.CreativeId,
                        principalTable: "VkOrdCreativeCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkOrdCreativeMedia_VkOrdMediaCache_MediaId",
                        column: x => x.MediaId,
                        principalTable: "VkOrdMediaCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ApiCredentialId",
                table: "VkOrdContractCache",
                column: "ApiCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ApiCredentialId_ExternalId",
                table: "VkOrdContractCache",
                columns: new[] { "ApiCredentialId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_CachedAt",
                table: "VkOrdContractCache",
                column: "CachedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ClientExternalId",
                table: "VkOrdContractCache",
                column: "ClientExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ContractorExternalId",
                table: "VkOrdContractCache",
                column: "ContractorExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_Date",
                table: "VkOrdContractCache",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_DateEnd",
                table: "VkOrdContractCache",
                column: "DateEnd");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ExpiresAt",
                table: "VkOrdContractCache",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_ParentContractId",
                table: "VkOrdContractCache",
                column: "ParentContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractCache_SyncStatus",
                table: "VkOrdContractCache",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParty_ContractId",
                table: "VkOrdContractParty",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParty_CounterpartyId",
                table: "VkOrdContractParty",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParty_CreatedAt",
                table: "VkOrdContractParty",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParty_Role",
                table: "VkOrdContractParty",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdContractParty_VkOrdCounterpartyCacheId",
                table: "VkOrdContractParty",
                column: "VkOrdCounterpartyCacheId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_ApiCredentialId",
                table: "VkOrdCounterpartyCache",
                column: "ApiCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_ApiCredentialId_ExternalId",
                table: "VkOrdCounterpartyCache",
                columns: new[] { "ApiCredentialId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_CachedAt",
                table: "VkOrdCounterpartyCache",
                column: "CachedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_ExpiresAt",
                table: "VkOrdCounterpartyCache",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_Inn",
                table: "VkOrdCounterpartyCache",
                column: "Inn");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_Name",
                table: "VkOrdCounterpartyCache",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyCache_SyncStatus",
                table: "VkOrdCounterpartyCache",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyRelation_CreatedAt",
                table: "VkOrdCounterpartyRelation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyRelation_FromCounterpartyId",
                table: "VkOrdCounterpartyRelation",
                column: "FromCounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyRelation_RelationType",
                table: "VkOrdCounterpartyRelation",
                column: "RelationType");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCounterpartyRelation_ToCounterpartyId",
                table: "VkOrdCounterpartyRelation",
                column: "ToCounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_ApiCredentialId",
                table: "VkOrdCreativeCache",
                column: "ApiCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_ApiCredentialId_ExternalId",
                table: "VkOrdCreativeCache",
                columns: new[] { "ApiCredentialId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_CachedAt",
                table: "VkOrdCreativeCache",
                column: "CachedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_Erid",
                table: "VkOrdCreativeCache",
                column: "Erid");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_ExpiresAt",
                table: "VkOrdCreativeCache",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_Name",
                table: "VkOrdCreativeCache",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_PersonExternalId",
                table: "VkOrdCreativeCache",
                column: "PersonExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_Status",
                table: "VkOrdCreativeCache",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdCreativeCache_SyncStatus",
                table: "VkOrdCreativeCache",
                column: "SyncStatus");

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
                name: "IX_VkOrdMediaCache_ApiCredentialId",
                table: "VkOrdMediaCache",
                column: "ApiCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_ApiCredentialId_ExternalId",
                table: "VkOrdMediaCache",
                columns: new[] { "ApiCredentialId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_CachedAt",
                table: "VkOrdMediaCache",
                column: "CachedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_ContentType",
                table: "VkOrdMediaCache",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_ExpiresAt",
                table: "VkOrdMediaCache",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_Filename",
                table: "VkOrdMediaCache",
                column: "Filename");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_MediaType",
                table: "VkOrdMediaCache",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_Sha256",
                table: "VkOrdMediaCache",
                column: "Sha256");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_SyncStatus",
                table: "VkOrdMediaCache",
                column: "SyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdMediaCache_UploadStatus",
                table: "VkOrdMediaCache",
                column: "UploadStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_ApiCredentialId",
                table: "VkOrdStatisticsCache",
                column: "ApiCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_ApiCredentialId_ExternalId",
                table: "VkOrdStatisticsCache",
                columns: new[] { "ApiCredentialId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_CachedAt",
                table: "VkOrdStatisticsCache",
                column: "CachedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_CreativeExternalId",
                table: "VkOrdStatisticsCache",
                column: "CreativeExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_CreativeId",
                table: "VkOrdStatisticsCache",
                column: "CreativeId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_DateEndActual",
                table: "VkOrdStatisticsCache",
                column: "DateEndActual");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_DateEndPlanned",
                table: "VkOrdStatisticsCache",
                column: "DateEndPlanned");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_DateStartActual",
                table: "VkOrdStatisticsCache",
                column: "DateStartActual");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_DateStartPlanned",
                table: "VkOrdStatisticsCache",
                column: "DateStartPlanned");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_ExpiresAt",
                table: "VkOrdStatisticsCache",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_PadExternalId",
                table: "VkOrdStatisticsCache",
                column: "PadExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_Period",
                table: "VkOrdStatisticsCache",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_StatisticsType",
                table: "VkOrdStatisticsCache",
                column: "StatisticsType");

            migrationBuilder.CreateIndex(
                name: "IX_VkOrdStatisticsCache_SyncStatus",
                table: "VkOrdStatisticsCache",
                column: "SyncStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkOrdContractParty");

            migrationBuilder.DropTable(
                name: "VkOrdCounterpartyRelation");

            migrationBuilder.DropTable(
                name: "VkOrdCreativeContract");

            migrationBuilder.DropTable(
                name: "VkOrdCreativeMedia");

            migrationBuilder.DropTable(
                name: "VkOrdStatisticsCache");

            migrationBuilder.DropTable(
                name: "VkOrdCounterpartyCache");

            migrationBuilder.DropTable(
                name: "VkOrdContractCache");

            migrationBuilder.DropTable(
                name: "VkOrdMediaCache");

            migrationBuilder.DropTable(
                name: "VkOrdCreativeCache");
        }
    }
}
