using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnix.Migrations
{
    /// <inheritdoc />
    public partial class PerfilNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Torna a FK PerfilDeAprendizagemId nullable em Alunos.
            // Isso permite criar alunos sem perfil pre-definido.
            // O perfil sera criado somente quando o aluno personalizar sua experiencia.
            migrationBuilder.AlterColumn<int>(
                name: "PerfilDeAprendizagemId",
                table: "Alunos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_PerfilDeAprendizagemId",
                table: "Alunos");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_PerfilDeAprendizagemId",
                table: "Alunos",
                column: "PerfilDeAprendizagemId",
                unique: true,
                filter: "[PerfilDeAprendizagemId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PerfilDeAprendizagemId",
                table: "Alunos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldNullable: true,
                oldType: "int");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_PerfilDeAprendizagemId",
                table: "Alunos");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_PerfilDeAprendizagemId",
                table: "Alunos",
                column: "PerfilDeAprendizagemId",
                unique: true,
                filter: "[PerfilDeAprendizagemId] IS NOT NULL");
        }
    }
}
