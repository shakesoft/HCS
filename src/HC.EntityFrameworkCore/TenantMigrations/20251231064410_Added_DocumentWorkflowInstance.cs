using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Added_DocumentWorkflowInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDocumentWorkflowInstances",
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
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDocumentWorkflowInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDocumentWorkflowInstances_AppDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "AppDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppDocumentWorkflowInstances_AppWorkflowStepTemplates_Curre~",
                        column: x => x.CurrentStepId,
                        principalTable: "AppWorkflowStepTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppDocumentWorkflowInstances_AppWorkflowTemplates_WorkflowT~",
                        column: x => x.WorkflowTemplateId,
                        principalTable: "AppWorkflowTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppDocumentWorkflowInstances_AppWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "AppWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDocumentWorkflowInstances_CurrentStepId",
                table: "AppDocumentWorkflowInstances",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocumentWorkflowInstances_DocumentId",
                table: "AppDocumentWorkflowInstances",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocumentWorkflowInstances_WorkflowId",
                table: "AppDocumentWorkflowInstances",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDocumentWorkflowInstances_WorkflowTemplateId",
                table: "AppDocumentWorkflowInstances",
                column: "WorkflowTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDocumentWorkflowInstances");
        }
    }
}
