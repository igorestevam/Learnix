using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnix.Migrations
{
    public partial class RemoveAulasConcluidasDeProgresso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AulasConcluidas",
                table: "Progressos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AulasConcluidas",
                table: "Progressos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
