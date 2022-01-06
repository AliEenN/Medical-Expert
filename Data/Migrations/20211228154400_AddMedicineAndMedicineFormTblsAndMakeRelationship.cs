using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalExpert.Data.Migrations
{
    public partial class AddMedicineAndMedicineFormTblsAndMakeRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicinesAndMedicineForms",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    MedicineFormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinesAndMedicineForms", x => new { x.MedicineId, x.MedicineFormId });
                    table.ForeignKey(
                        name: "FK_MedicinesAndMedicineForms_MedicineForms_MedicineFormId",
                        column: x => x.MedicineFormId,
                        principalTable: "MedicineForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicinesAndMedicineForms_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicinesAndMedicineForms_MedicineFormId",
                table: "MedicinesAndMedicineForms",
                column: "MedicineFormId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicinesAndMedicineForms");
        }
    }
}
