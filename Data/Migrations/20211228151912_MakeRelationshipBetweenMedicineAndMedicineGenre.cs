using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalExpert.Data.Migrations
{
    public partial class MakeRelationshipBetweenMedicineAndMedicineGenre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicineGenreId",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_MedicineGenreId",
                table: "Medicines",
                column: "MedicineGenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_MedicineGenres_MedicineGenreId",
                table: "Medicines",
                column: "MedicineGenreId",
                principalTable: "MedicineGenres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_MedicineGenres_MedicineGenreId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_MedicineGenreId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "MedicineGenreId",
                table: "Medicines");
        }
    }
}
