using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Updated_Document_26011116021754 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecrecyLevel",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "UrgencyLevel",
                table: "AppDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "SignType",
                table: "AppUserSignatures",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderType",
                table: "AppSignatureSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultSignType",
                table: "AppSignatureSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "SecrecyLevelId",
                table: "AppDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "AppDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UrgencyLevelId",
                table: "AppDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Visibility",
                table: "AppCalendarEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedType",
                table: "AppCalendarEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "AppCalendarEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ResponseStatus",
                table: "AppCalendarEventParticipants",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_SecrecyLevelId",
                table: "AppDocuments",
                column: "SecrecyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_TypeId",
                table: "AppDocuments",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_UrgencyLevelId",
                table: "AppDocuments",
                column: "UrgencyLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_SecrecyLevelId",
                table: "AppDocuments",
                column: "SecrecyLevelId",
                principalTable: "AppMasterDatas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_TypeId",
                table: "AppDocuments",
                column: "TypeId",
                principalTable: "AppMasterDatas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_UrgencyLevelId",
                table: "AppDocuments",
                column: "UrgencyLevelId",
                principalTable: "AppMasterDatas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_SecrecyLevelId",
                table: "AppDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_TypeId",
                table: "AppDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDocuments_AppMasterDatas_UrgencyLevelId",
                table: "AppDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AppDocuments_SecrecyLevelId",
                table: "AppDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AppDocuments_TypeId",
                table: "AppDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AppDocuments_UrgencyLevelId",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "SecrecyLevelId",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "UrgencyLevelId",
                table: "AppDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "SignType",
                table: "AppUserSignatures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ProviderType",
                table: "AppSignatureSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DefaultSignType",
                table: "AppSignatureSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "SecrecyLevel",
                table: "AppDocuments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AppDocuments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrgencyLevel",
                table: "AppDocuments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                table: "AppCalendarEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "RelatedType",
                table: "AppCalendarEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "EventType",
                table: "AppCalendarEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ResponseStatus",
                table: "AppCalendarEventParticipants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
