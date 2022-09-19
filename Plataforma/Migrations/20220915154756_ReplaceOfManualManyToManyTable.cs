using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Migrations
{
    public partial class ReplaceOfManualManyToManyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkSheetsMaterials");

            migrationBuilder.AlterColumn<int>(
                name: "Telephone",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Telephone",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "MaterialWorkSheet",
                columns: table => new
                {
                    MaterialsId = table.Column<int>(type: "int", nullable: false),
                    WorkSheetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorkSheet", x => new { x.MaterialsId, x.WorkSheetsId });
                    table.ForeignKey(
                        name: "FK_MaterialWorkSheet_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialWorkSheet_WorkSheets_WorkSheetsId",
                        column: x => x.WorkSheetsId,
                        principalTable: "WorkSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorkSheet_WorkSheetsId",
                table: "MaterialWorkSheet",
                column: "WorkSheetsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialWorkSheet");

            migrationBuilder.AlterColumn<int>(
                name: "Telephone",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Telephone",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WorkSheetsMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    WorkSheetId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSheetsMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSheetsMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkSheetsMaterials_WorkSheets_WorkSheetId",
                        column: x => x.WorkSheetId,
                        principalTable: "WorkSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkSheetsMaterials_MaterialId",
                table: "WorkSheetsMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSheetsMaterials_WorkSheetId",
                table: "WorkSheetsMaterials",
                column: "WorkSheetId");
        }
    }
}
