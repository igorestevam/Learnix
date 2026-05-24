using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnix.Migrations
{
    /// <inheritdoc />
    public partial class LearnixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Cursos");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "Usuarios",
                newName: "TipoUsuario");

            migrationBuilder.AddColumn<string>(
                name: "TipoCurso",
                table: "Cursos",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCurso",
                table: "Cursos");

            migrationBuilder.RenameColumn(
                name: "TipoUsuario",
                table: "Usuarios",
                newName: "Discriminator");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Cursos",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }
    }
}
