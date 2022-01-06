using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalExpert.Data.Migrations
{
    public partial class AddDiseaseRiskColumnToDiseasesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiseaseRisk",
                table: "Diseases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiseaseRisk",
                table: "Diseases");
        }
    }
}
