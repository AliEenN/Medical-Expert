using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalExpert.Data.Migrations
{
    public partial class AddCategorySideTblAndMakeRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriesSides",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SideId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesSides", x => new { x.CategoryId, x.SideId });
                    table.ForeignKey(
                        name: "FK_CategoriesSides_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriesSides_Sides_SideId",
                        column: x => x.SideId,
                        principalTable: "Sides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesSides_SideId",
                table: "CategoriesSides",
                column: "SideId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesSides");
        }
    }
}
