using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HC.TenantMigrations
{
    /// <inheritdoc />
    public partial class Updated_Document_26011116043084 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageNumber",
                table: "AppDocuments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageNumber",
                table: "AppDocuments");
        }
    }
}
