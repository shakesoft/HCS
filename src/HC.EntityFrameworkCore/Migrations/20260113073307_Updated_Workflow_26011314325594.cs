using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Workflow_26011314325594 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowDefinitionId",
                table: "AppWorkflows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflows_WorkflowDefinitionId",
                table: "AppWorkflows",
                column: "WorkflowDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkflows_AppWorkflowDefinitions_WorkflowDefinitionId",
                table: "AppWorkflows",
                column: "WorkflowDefinitionId",
                principalTable: "AppWorkflowDefinitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkflows_AppWorkflowDefinitions_WorkflowDefinitionId",
                table: "AppWorkflows");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkflows_WorkflowDefinitionId",
                table: "AppWorkflows");

            migrationBuilder.DropColumn(
                name: "WorkflowDefinitionId",
                table: "AppWorkflows");
        }
    }
}
