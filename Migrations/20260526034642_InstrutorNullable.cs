using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnix.Migrations
{
    /// <inheritdoc />
    public partial class InstrutorNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_Usuarios_InstrutorId",
                table: "Cursos");

            migrationBuilder.AlterColumn<int>(
                name: "InstrutorId",
                table: "Cursos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_Usuarios_InstrutorId",
                table: "Cursos",
                column: "InstrutorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_Usuarios_InstrutorId",
                table: "Cursos");

            migrationBuilder.AlterColumn<int>(
                name: "InstrutorId",
                table: "Cursos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_Usuarios_InstrutorId",
                table: "Cursos",
                column: "InstrutorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
