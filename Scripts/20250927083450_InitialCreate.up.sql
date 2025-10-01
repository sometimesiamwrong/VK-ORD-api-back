CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "UserName" character varying(100) NOT NULL,
    "Name" character varying(200),
    "PasswordHash" character varying(500) NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "RowVersion" bytea NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "ApiCredentials" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Environment" character varying(20) NOT NULL,
    "TokenEncrypted" text NOT NULL,
    "DisplayName" character varying(150),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "RowVersion" bytea NOT NULL,
    CONSTRAINT "PK_ApiCredentials" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ApiCredentials_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RefreshTokens" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "TokenHash" character varying(256) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedByIp" character varying(64),
    "DeviceId" character varying(128),
    "RevokedAt" timestamp with time zone,
    "ReplacedByTokenHash" character varying(256),
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ApiCredentials_UserId" ON "ApiCredentials" ("UserId");

CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");

CREATE UNIQUE INDEX "IX_Users_UserName" ON "Users" ("UserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250927083450_InitialCreate', '8.0.8');

COMMIT;

START TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250930154619_FixRowVersionConfiguration', '8.0.8');

COMMIT;

