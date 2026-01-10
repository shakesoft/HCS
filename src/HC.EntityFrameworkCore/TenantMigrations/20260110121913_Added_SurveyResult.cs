using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Added_SurveyResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSurveyResults",
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
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    SurveyCriteriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveySessionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSurveyResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSurveyResults_AppSurveyCriterias_SurveyCriteriaId",
                        column: x => x.SurveyCriteriaId,
                        principalTable: "AppSurveyCriterias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppSurveyResults_AppSurveySessions_SurveySessionId",
                        column: x => x.SurveySessionId,
                        principalTable: "AppSurveySessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSurveyResults_SurveyCriteriaId",
                table: "AppSurveyResults",
                column: "SurveyCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSurveyResults_SurveySessionId",
                table: "AppSurveyResults",
                column: "SurveySessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSurveyResults");
        }
    }
}
