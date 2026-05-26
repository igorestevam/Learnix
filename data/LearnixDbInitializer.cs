using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.model;

namespace Learnix.data
{
    public static class LearnixDbInitializer
    {
        public static void Seed(LearnixDbContext ctx)
        {
            ctx.Database.Migrate();

            SeedCategorias(ctx);
            SeedInstrutor(ctx);
            SeedAlunoDemo(ctx);
            SeedCursos(ctx);
        }

        private static void SeedCategorias(LearnixDbContext ctx)
        {
            if (ctx.Categorias.Any()) return;

            ctx.Categorias.AddRange(
                new Categoria { Nome = "Exatas",     Descricao = "Cursos da area de exatas" },
                new Categoria { Nome = "Humanas",    Descricao = "Cursos da area de humanas" },
                new Categoria { Nome = "Tecnologia", Descricao = "Cursos da area de tecnologia" }
            );
            ctx.SaveChanges();
        }

        private static void SeedInstrutor(LearnixDbContext ctx)
        {
            if (ctx.Instrutores.Any()) return;

            ctx.Instrutores.Add(new Instrutor
            {
                Nome          = "Prof. Ricardo Almeida",
                Email         = "ricardo@learnix.com",
                Senha         = "123456",
                DataCadastro  = DateTime.Now,
                Especialidade = "Ciencia da Computacao",
                Biografia     = "Professor com 15 anos de experiencia em ensino de programacao."
            });
            ctx.SaveChanges();
        }

        private static void SeedAlunoDemo(LearnixDbContext ctx)
        {
            if (ctx.Alunos.Any(a => a.MatriculaAcademica == "DEMO001")) return;

            // Cria o aluno demo LIMPO - sem perfil, sem matriculas, sem historico pre-preenchido
            ctx.Alunos.Add(new Aluno
            {
                Nome               = "Aluno Demo",
                Email              = "demo@learnix.com",
                Senha              = "123456",
                DataCadastro       = DateTime.Now,
                MatriculaAcademica = "DEMO001"
            });
            ctx.SaveChanges();
        }
        private static void SeedCursos(LearnixDbContext ctx)
        {
            if (ctx.Cursos.Any()) return;

            var catExatas  = ctx.Categorias.First(c => c.Nome == "Exatas");
            var catHumanas = ctx.Categorias.First(c => c.Nome == "Humanas");
            var catTec     = ctx.Categorias.First(c => c.Nome == "Tecnologia");
            var instrutor  = ctx.Instrutores.First();

            var c1 = new CursoExatas
            {
                Titulo        = "Algoritmos e Estrutura de Dados",
                Descricao     = "Aprenda os fundamentos de algoritmos e estruturas de dados essenciais para a programacao.",
                CargaHoraria  = 60,
                Preco         = 199.90m,
                CategoriaId   = catExatas.Id,
                InstrutorId   = instrutor.Id
            };

            var c2 = new CursoExatas
            {
                Titulo        = "Calculo I - Limites e Derivadas",
                Descricao     = "Introducao ao calculo diferencial, com foco em limites, continuidade e derivadas.",
                CargaHoraria  = 80,
                Preco         = 249.90m,
                CategoriaId   = catExatas.Id,
                InstrutorId   = instrutor.Id
            };

            var c3 = new CursoHumanas
            {
                Titulo        = "Engenharia de Software",
                Descricao     = "Principios e praticas para o desenvolvimento profissional de software.",
                CargaHoraria  = 50,
                Preco         = 219.90m,
                CategoriaId   = catHumanas.Id,
                InstrutorId   = instrutor.Id
            };

            var c4 = new CursoExatas
            {
                Titulo        = "Banco de Dados Relacional",
                Descricao     = "Modelagem, normalizacao e consultas SQL com bancos relacionais.",
                CargaHoraria  = 45,
                Preco         = 189.90m,
                CategoriaId   = catTec.Id,
                InstrutorId   = instrutor.Id
            };

            var c5 = new CursoExatas
            {
                Titulo        = "Programacao Orientada a Objetos em C#",
                Descricao     = "Domine OOP, padroes de projeto e boas praticas com a linguagem C#.",
                CargaHoraria  = 70,
                Preco         = 259.90m,
                CategoriaId   = catTec.Id,
                InstrutorId   = instrutor.Id
            };

            ctx.Cursos.AddRange(c1, c2, c3, c4, c5);
            ctx.SaveChanges();

            SeedModulosCurso(ctx, c1.Id, new (string, (string, string, int)[])[]
            {
                ("Modulo 1 - Introducao", new (string, string, int)[] {
                    ("Aula 01 - O que sao Algoritmos?",        "", 15),
                    ("Aula 02 - Complexidade de Algoritmos",   "", 20),
                    ("Aula 03 - Logica Estruturada",           "", 25)
                }),
                ("Modulo 2 - Estruturas", new (string, string, int)[] {
                    ("Aula 04 - Listas",       "", 20),
                    ("Aula 05 - Pilhas e Filas", "", 25),
                    ("Aula 06 - Arvores",       "", 30)
                })
            });

            SeedModulosCurso(ctx, c2.Id, new (string, (string, string, int)[])[]
            {
                ("Modulo 1 - Limites", new (string, string, int)[] {
                    ("Aula 01 - Conceito de Limite",  "", 30),
                    ("Aula 02 - Limites Laterais",    "", 25)
                }),
                ("Modulo 2 - Derivadas", new (string, string, int)[] {
                    ("Aula 03 - Definicao de Derivada", "", 30)
                })
            });

            SeedModulosCurso(ctx, c3.Id, new (string, (string, string, int)[])[]
            {
                ("Modulo 1 - Engenharia de Software", new (string, string, int)[] {
                    ("Aula 01 - Processos de Software",  "", 25),
                    ("Aula 02 - Metodologias Ageis",     "", 30),
                    ("Aula 03 - Qualidade de Software",  "", 20)
                })
            });

            SeedModulosCurso(ctx, c4.Id, new (string, (string, string, int)[])[]
            {
                ("Modulo 1 - Modelagem", new (string, string, int)[] {
                    ("Aula 01 - Modelo Entidade-Relacionamento", "", 30),
                    ("Aula 02 - Normalizacao",                   "", 25)
                }),
                ("Modulo 2 - SQL", new (string, string, int)[] {
                    ("Aula 03 - SELECT e JOINs", "", 30),
                    ("Aula 04 - Subqueries",     "", 25)
                })
            });

            SeedModulosCurso(ctx, c5.Id, new (string, (string, string, int)[])[]
            {
                ("Modulo 1 - Fundamentos OOP", new (string, string, int)[] {
                    ("Aula 01 - Classes e Objetos",  "", 25),
                    ("Aula 02 - Encapsulamento",     "", 20),
                    ("Aula 03 - Heranca",            "", 25)
                }),
                ("Modulo 2 - Padroes", new (string, string, int)[] {
                    ("Aula 04 - Polimorfismo",            "", 30),
                    ("Aula 05 - Interfaces e Abstracao",  "", 25)
                })
            });
        }

        private static void SeedModulosCurso(LearnixDbContext ctx, int cursoId, (string Titulo, (string Titulo, string VideoUrl, int Minutos)[] Aulas)[] modulos)
        {
            int ordemMod = 1;
            foreach (var (tituloMod, aulas) in modulos)
            {
                var modulo = new Modulo
                {
                    Titulo   = tituloMod,
                    Ordem    = ordemMod++,
                    CursoId  = cursoId
                };
                ctx.Modulos.Add(modulo);
                ctx.SaveChanges();

                int ordemAula = 1;
                foreach (var (tituloAula, videoUrl, minutos) in aulas)
                {
                    ctx.Aulas.Add(new Aula
                    {
                        Titulo    = tituloAula,
                        VideoUrl  = videoUrl,
                        Duracao   = TimeSpan.FromMinutes(minutos),
                        Ordem     = ordemAula++,
                        ModuloId  = modulo.Id
                    });
                }
                ctx.SaveChanges();
            }
        }
    }
}
