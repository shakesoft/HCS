using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.Migrations
{
    /// <inheritdoc />
    public partial class Added_SignatureSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSignatureSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProviderCode = table.Column<string>(type: "text", nullable: false),
                    ProviderType = table.Column<string>(type: "text", nullable: false),
                    ApiEndpoint = table.Column<string>(type: "text", nullable: false),
                    ApiTimeout = table.Column<int>(type: "integer", nullable: false),
                    DefaultSignType = table.Column<string>(type: "text", nullable: false),
                    AllowElectronicSign = table.Column<bool>(type: "boolean", nullable: false),
                    AllowDigitalSign = table.Column<bool>(type: "boolean", nullable: false),
                    RequireOtp = table.Column<bool>(type: "boolean", nullable: false),
                    SignWidth = table.Column<int>(type: "integer", nullable: false),
                    SignHeight = table.Column<int>(type: "integer", nullable: false),
                    SignedFileSuffix = table.Column<string>(type: "text", nullable: false),
                    KeepOriginalFile = table.Column<bool>(type: "boolean", nullable: false),
                    OverwriteSignedFile = table.Column<bool>(type: "boolean", nullable: false),
                    EnableSignLog = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSignatureSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSignatureSettings");
        }
    }
}
