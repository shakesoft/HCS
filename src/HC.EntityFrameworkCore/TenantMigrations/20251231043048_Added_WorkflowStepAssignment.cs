using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Added_WorkflowStepAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppWorkflowStepAssignments",
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
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: true),
                    StepId = table.Column<Guid>(type: "uuid", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefaultUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWorkflowStepAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWorkflowStepAssignments_AbpUsers_DefaultUserId",
                        column: x => x.DefaultUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppWorkflowStepAssignments_AppWorkflowStepTemplates_StepId",
                        column: x => x.StepId,
                        principalTable: "AppWorkflowStepTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppWorkflowStepAssignments_AppWorkflowTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "AppWorkflowTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppWorkflowStepAssignments_AppWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "AppWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_DefaultUserId",
                table: "AppWorkflowStepAssignments",
                column: "DefaultUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_StepId",
                table: "AppWorkflowStepAssignments",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_TemplateId",
                table: "AppWorkflowStepAssignments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_WorkflowId",
                table: "AppWorkflowStepAssignments",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWorkflowStepAssignments");
        }
    }
}
