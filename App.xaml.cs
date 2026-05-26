using System.Windows;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SeedBanco();
        }

        private void SeedBanco()
        {
            using var db = new LearnixDbContext();

            // Só popula se ainda não tiver cursos
            if (db.Cursos.Any()) return;

            // Categorias
            var exatas = new Categoria { Nome = "Exatas", Descricao = "Ciências Exatas" };
            var humanas = new Categoria { Nome = "Humanas", Descricao = "Ciências Humanas" };
            var tecnologia = new Categoria { Nome = "Tecnologia", Descricao = "Tecnologia da Informação" };
            db.Categorias.AddRange(exatas, humanas, tecnologia);
            db.SaveChanges();

            // Cursos sem instrutor — instrutor se candidata pela tela
            var cursosExatas = new CursoExatas[]
            {
                new CursoExatas
                {
                    Titulo = "Algoritmos e Estrutura de Dados",
                    Descricao = "Aprenda os fundamentos de algoritmos, complexidade e as principais estruturas de dados usadas na programação.",
                    CargaHoraria = 60, Preco = 0, CategoriaId = exatas.Id,
                    PossuiLaboratorioVirtual = true, FerramentaSoftwareSugerida = "VSCode"
                },
                new CursoExatas
                {
                    Titulo = "Cálculo I",
                    Descricao = "Limites, derivadas e integrais com aplicações práticas para engenharia e ciências exatas.",
                    CargaHoraria = 45, Preco = 0, CategoriaId = exatas.Id,
                    PossuiLaboratorioVirtual = false, FerramentaSoftwareSugerida = "GeoGebra"
                },
            };
            db.CursosExatas.AddRange(cursosExatas);

            var cursosHumanas = new CursoHumanas[]
            {
                new CursoHumanas
                {
                    Titulo = "Engenharia de Software",
                    Descricao = "Metodologias ágeis, padrões de projeto, testes e boas práticas de desenvolvimento de software.",
                    CargaHoraria = 80, Preco = 0, CategoriaId = humanas.Id,
                    ExigeMonografia = false, QuantidadeLivrosObrigatorios = 2
                },
            };
            db.CursosHumanas.AddRange(cursosHumanas);

            var cursosTecnologia = new CursoExatas[]
            {
                new CursoExatas
                {
                    Titulo = "Banco de Dados Relacional",
                    Descricao = "Modelagem, SQL, normalização e administração de bancos de dados relacionais com PostgreSQL e SQL Server.",
                    CargaHoraria = 55, Preco = 0, CategoriaId = tecnologia.Id,
                    PossuiLaboratorioVirtual = true, FerramentaSoftwareSugerida = "PostgreSQL"
                },
                new CursoExatas
                {
                    Titulo = "Programação Orientada a Objetos em C#",
                    Descricao = "Classes, herança, polimorfismo, interfaces e padrões de projeto com C# e .NET.",
                    CargaHoraria = 40, Preco = 0, CategoriaId = tecnologia.Id,
                    PossuiLaboratorioVirtual = true, FerramentaSoftwareSugerida = "Visual Studio"
                },
            };
            db.CursosExatas.AddRange(cursosTecnologia);

            db.SaveChanges();
        }
    }
}
