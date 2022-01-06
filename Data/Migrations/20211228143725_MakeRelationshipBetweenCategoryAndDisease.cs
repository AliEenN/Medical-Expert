using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalExpert.Data.Migrations
{
    public partial class MakeRelationshipBetweenCategoryAndDisease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Diseases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_CategoryId",
                table: "Diseases",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diseases_Categories_CategoryId",
                table: "Diseases",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diseases_Categories_CategoryId",
                table: "Diseases");

            migrationBuilder.DropIndex(
                name: "IX_Diseases_CategoryId",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Diseases");
        }
    }
}
