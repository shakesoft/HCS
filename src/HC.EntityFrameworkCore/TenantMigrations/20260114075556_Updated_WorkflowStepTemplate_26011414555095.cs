using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Updated_WorkflowStepTemplate_26011414555095 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkflowStepTemplates_AppWorkflows_WorkflowId",
                table: "AppWorkflowStepTemplates");

            migrationBuilder.RenameColumn(
                name: "WorkflowId",
                table: "AppWorkflowStepTemplates",
                newName: "WorkflowTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_AppWorkflowStepTemplates_WorkflowId",
                table: "AppWorkflowStepTemplates",
                newName: "IX_AppWorkflowStepTemplates_WorkflowTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkflowStepTemplates_AppWorkflowTemplates_WorkflowTempl~",
                table: "AppWorkflowStepTemplates",
                column: "WorkflowTemplateId",
                principalTable: "AppWorkflowTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkflowStepTemplates_AppWorkflowTemplates_WorkflowTempl~",
                table: "AppWorkflowStepTemplates");

            migrationBuilder.RenameColumn(
                name: "WorkflowTemplateId",
                table: "AppWorkflowStepTemplates",
                newName: "WorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_AppWorkflowStepTemplates_WorkflowTemplateId",
                table: "AppWorkflowStepTemplates",
                newName: "IX_AppWorkflowStepTemplates_WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkflowStepTemplates_AppWorkflows_WorkflowId",
                table: "AppWorkflowStepTemplates",
                column: "WorkflowId",
                principalTable: "AppWorkflows",
                principalColumn: "Id");
        }
    }
}
