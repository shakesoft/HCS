using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Updated_WorkflowStepAssignment_26011414524319 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkflowStepAssignments_AppWorkflowTemplates_TemplateId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkflowStepAssignments_AppWorkflows_WorkflowId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkflowStepAssignments_TemplateId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkflowStepAssignments_WorkflowId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "AppWorkflowStepAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId",
                table: "AbpUsers",
                type: "uuid",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "AbpUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "AppWorkflowStepAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "AppWorkflowStepAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_TemplateId",
                table: "AppWorkflowStepAssignments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStepAssignments_WorkflowId",
                table: "AppWorkflowStepAssignments",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkflowStepAssignments_AppWorkflowTemplates_TemplateId",
                table: "AppWorkflowStepAssignments",
                column: "TemplateId",
                principalTable: "AppWorkflowTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkflowStepAssignments_AppWorkflows_WorkflowId",
                table: "AppWorkflowStepAssignments",
                column: "WorkflowId",
                principalTable: "AppWorkflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
