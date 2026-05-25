using System;
using System.Linq;
using Learnix.model;

namespace Learnix.data
{
    /// <summary>
    /// Popula o banco com dados iniciais (categorias, perfis, instrutor demo e cursos demo)
    /// para que as telas tenham conteudo ao iniciar. Idempotente: so insere se a tabela estiver vazia.
    /// Chamado no App.xaml.cs no startup.
    /// </summary>
    public static class LearnixDbInitializer
    {
        public static void Seed(LearnixDbContext context)
        {
            // Garante que o banco existe (aplica migrations pendentes)
            context.Database.EnsureCreated();

            SeedCategorias(context);
            SeedPerfis(context);
            SeedInstrutorDemo(context);
            SeedCursosDemo(context);
            SeedAlunoDemo(context);
        }

        private static void SeedCategorias(LearnixDbContext context)
        {
            if (context.Categorias.Any()) return;

            context.Categorias.AddRange(
                new Categoria(0, "Exatas", "Cursos de Matematica, Fisica, Engenharia"),
                new Categoria(0, "Humanas", "Cursos de Letras, Historia, Filosofia"),
                new Categoria(0, "Tecnologia", "Cursos de Programacao, Redes, Dados")
            );
            context.SaveChanges();
        }

        private static void SeedPerfis(LearnixDbContext context)
        {
            if (context.PerfisDeAprendizagem.Any()) return;

            context.PerfisDeAprendizagem.AddRange(
                new PerfilDeAprendizagem(0, "Visual", "Regular"),
                new PerfilDeAprendizagem(0, "Auditivo", "Flexivel"),
                new PerfilDeAprendizagem(0, "Leitura/Escrita", "Intensivo"),
                new PerfilDeAprendizagem(0, "Cinestesico", "Regular")
            );
            context.SaveChanges();
        }

        private static void SeedInstrutorDemo(LearnixDbContext context)
        {
            if (context.Instrutores.Any()) return;

            Instrutor inst = new Instrutor
            {
                Nome = "Prof. Ricardo Almeida",
                Email = "ricardo@learnix.com",
                Senha = "123456",
                Especialidade = "Engenharia de Software",
                Biografia = "Mestre em Computacao, 10 anos de docencia"
            };
            context.Instrutores.Add(inst);
            context.SaveChanges();
        }

        private static void SeedCursosDemo(LearnixDbContext context)
        {
            if (context.Cursos.Any()) return;

            var catTec = context.Categorias.FirstOrDefault(c => c.Nome == "Tecnologia");
            var catExatas = context.Categorias.FirstOrDefault(c => c.Nome == "Exatas");
            var catHumanas = context.Categorias.FirstOrDefault(c => c.Nome == "Humanas");
            var inst = context.Instrutores.FirstOrDefault();

            if (catTec == null || catExatas == null || catHumanas == null || inst == null) return;

            // Curso 1 - Tecnologia
            CursoExatas curso1 = new CursoExatas
            {
                Titulo = "Logica de Programacao com C#",
                Descricao = "Aprenda os fundamentos da programacao usando C# e .NET",
                CargaHoraria = 40,
                Preco = 199.90m,
                CategoriaId = catTec.Id,
                InstrutorId = inst.Id,
                PossuiLaboratorioVirtual = true,
                FerramentaSoftwareSugerida = "Visual Studio 2022"
            };

            // Curso 2 - Exatas
            CursoExatas curso2 = new CursoExatas
            {
                Titulo = "Calculo I - Limites e Derivadas",
                Descricao = "Domine os conceitos fundamentais do calculo diferencial",
                CargaHoraria = 60,
                Preco = 249.90m,
                CategoriaId = catExatas.Id,
                InstrutorId = inst.Id,
                PossuiLaboratorioVirtual = false,
                FerramentaSoftwareSugerida = "GeoGebra"
            };

            // Curso 3 - Humanas
            CursoHumanas curso3 = new CursoHumanas
            {
                Titulo = "Introducao a Filosofia",
                Descricao = "Uma jornada pelos principais pensadores da historia",
                CargaHoraria = 30,
                Preco = 149.90m,
                CategoriaId = catHumanas.Id,
                InstrutorId = inst.Id,
                ExigeMonografia = true,
                QuantidadeLivrosObrigatorios = 5
            };

            context.Cursos.AddRange(curso1, curso2, curso3);
            context.SaveChanges();

            // Modulos e aulas para o curso 1
            SeedModulosAulas(context, curso1.Id, new[] {
                ("Modulo 1 - Introducao", new[] { "O que e C#", "Configurando o ambiente", "Primeiro programa" }),
                ("Modulo 2 - Variaveis e Tipos", new[] { "Tipos primitivos", "Operadores", "Conversao de tipos" }),
                ("Modulo 3 - Estruturas de Controle", new[] { "If/Else", "Loops", "Switch" })
            });

            SeedModulosAulas(context, curso2.Id, new[] {
                ("Modulo 1 - Limites", new[] { "Conceito de limite", "Limites laterais", "Limites no infinito" }),
                ("Modulo 2 - Derivadas", new[] { "Definicao de derivada", "Regras de derivacao", "Aplicacoes" })
            });

            SeedModulosAulas(context, curso3.Id, new[] {
                ("Modulo 1 - Filosofia Antiga", new[] { "Socrates", "Platao", "Aristoteles" }),
                ("Modulo 2 - Filosofia Moderna", new[] { "Descartes", "Kant", "Hegel" })
            });
        }

        private static void SeedModulosAulas(LearnixDbContext context, int cursoId, (string titulo, string[] aulas)[] estrutura)
        {
            int ordemModulo = 1;
            foreach (var (tituloModulo, aulas) in estrutura)
            {
                Modulo modulo = new Modulo
                {
                    Titulo = tituloModulo,
                    Ordem = ordemModulo,
                    CursoId = cursoId
                };
                context.Modulos.Add(modulo);
                context.SaveChanges();

                int ordemAula = 1;
                foreach (var tituloAula in aulas)
                {
                    Aula aula = new Aula
                    {
                        Titulo = tituloAula,
                        VideoUrl = "https://example.com/video.mp4",
                        Duracao = TimeSpan.FromMinutes(20),
                        Ordem = ordemAula,
                        ModuloId = modulo.Id
                    };
                    context.Aulas.Add(aula);
                    ordemAula++;
                }
                context.SaveChanges();
                ordemModulo++;
            }
        }

        private static void SeedAlunoDemo(LearnixDbContext context)
        {
            if (context.Alunos.Any()) return;

            var perfil = context.PerfisDeAprendizagem.FirstOrDefault();
            if (perfil == null) return;

            Aluno aluno = new Aluno
            {
                Nome = "Aluno Demo",
                Email = "demo@learnix.com",
                Senha = "123456",
                MatriculaAcademica = "DEMO001",
                PerfilDeAprendizagemId = perfil.Id
            };
            context.Alunos.Add(aluno);
            context.SaveChanges();
        }
    }
}
