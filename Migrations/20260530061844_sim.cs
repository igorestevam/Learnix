using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnix.Migrations
{
    /// <inheritdoc />
    public partial class sim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RespostasAtividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Resposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatriculaId = table.Column<int>(type: "int", nullable: false),
                    AtividadeCursoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasAtividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespostasAtividades_AtividadesCursos_AtividadeCursoId",
                        column: x => x.AtividadeCursoId,
                        principalTable: "AtividadesCursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespostasAtividades_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespostasAtividades_AtividadeCursoId",
                table: "RespostasAtividades",
                column: "AtividadeCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasAtividades_MatriculaId",
                table: "RespostasAtividades",
                column: "MatriculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespostasAtividades");
        }
    }
}
