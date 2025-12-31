using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.Migrations
{
    /// <inheritdoc />
    public partial class Added_Document : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDocuments",
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
                    No = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UrgencyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecrecyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrentStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: true),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDocuments_AppMasterDatas_FieldId",
                        column: x => x.FieldId,
                        principalTable: "AppMasterDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppDocuments_AppMasterDatas_StatusId",
                        column: x => x.StatusId,
                        principalTable: "AppMasterDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppDocuments_AppUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "AppUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppDocuments_AppWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "AppWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_FieldId",
                table: "AppDocuments",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_StatusId",
                table: "AppDocuments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_UnitId",
                table: "AppDocuments",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_WorkflowId",
                table: "AppDocuments",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDocuments");
        }
    }
}
