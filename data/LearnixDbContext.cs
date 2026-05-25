using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.data
{
    public class LearnixDbContext : DbContext
    {
        // ──────────────────────────────────────────────────────────────
        // DbSets — uma entrada por entidade concreta mapeada no banco
        // ──────────────────────────────────────────────────────────────

        // Hierarquia de Usuario (TPH → tabela unica "Usuarios" + coluna discriminador)
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Instrutor> Instrutores { get; set; } = null!;

        public DbSet<PerfilDeAprendizagem> PerfisDeAprendizagem { get; set; } = null!;

        // Hierarquia de Curso (TPH → tabela unica "Cursos" + coluna discriminador)
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<CursoExatas> CursosExatas { get; set; } = null!;
        public DbSet<CursoHumanas> CursosHumanas { get; set; } = null!;

        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;
        public DbSet<Modulo> Modulos { get; set; } = null!;
        public DbSet<Aula> Aulas { get; set; } = null!;
        public DbSet<Avaliacao> Avaliacoes { get; set; } = null!;
        public DbSet<Progresso> Progressos { get; set; } = null!;
        public DbSet<Certificado> Certificados { get; set; } = null!;

        // Tabela de juncao para registrar conclusoes unicas de aula por matricula
        public DbSet<AulaConcluida> AulasConcluidas { get; set; } = null!;

        // ──────────────────────────────────────────────────────────────
        // Construtores
        // ──────────────────────────────────────────────────────────────

        public LearnixDbContext() { }

        public LearnixDbContext(DbContextOptions<LearnixDbContext> options) : base(options) { }

        // ──────────────────────────────────────────────────────────────
        // Configuracao da conexao (fallback quando nao ha DI configurada)
        // ──────────────────────────────────────────────────────────────

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // TrustServerCertificate=True evita erro de certificado com
                // Microsoft.Data.SqlClient 4.x+ em ambiente de desenvolvimento local
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=Learnix;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Mapeamento do modelo
        // ──────────────────────────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── 1. TPH: hierarquia Usuario (Aluno / Instrutor) ────────
            // Tabela unica "Usuarios"; coluna "TipoUsuario" diferencia os tipos.
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("TipoUsuario")
                .HasValue<Aluno>("Aluno")
                .HasValue<Instrutor>("Instrutor");

            // ── 2. TPH: hierarquia Curso (CursoExatas / CursoHumanas) ─
            // Tabela unica "Cursos"; coluna "TipoCurso" diferencia os tipos.
            modelBuilder.Entity<Curso>()
                .HasDiscriminator<string>("TipoCurso")
                .HasValue<CursoExatas>("Exatas")
                .HasValue<CursoHumanas>("Humanas");

            // ── 3. Propriedade calculada — nao persiste no banco ──────
            // Matricula.NotaFinal e calculada em memoria; sem coluna no BD.
            modelBuilder.Entity<Matricula>()
                .Ignore(m => m.NotaFinal);

            // ── 4. Aluno → PerfilDeAprendizagem (1 para 1) ───────────
            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Perfil)
                .WithOne(p => p.Aluno)
                .HasForeignKey<Aluno>(a => a.PerfilDeAprendizagemId);

            // ── 5. Instrutor → Curso (1 para Muitos) ─────────────────
            modelBuilder.Entity<Instrutor>()
                .HasMany(i => i.Cursos)
                .WithOne(c => c.Instrutor)
                .HasForeignKey(c => c.InstrutorId);

            // ── 6. Aluno → Matricula (1 para Muitos) ─────────────────
            // Restrict: impede excluir um aluno que tenha historico de matriculas.
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Aluno)
                .WithMany(a => a.HistoricoMatriculas)
                .HasForeignKey(m => m.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── 7. Curso → Matricula (1 para Muitos) ─────────────────
            // Restrict: impede excluir um curso com alunos matriculados.
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Curso)
                .WithMany(c => c.MatriculasAtivas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── 8. Categoria → Curso (1 para Muitos) ─────────────────
            modelBuilder.Entity<Categoria>()
                .HasMany(cat => cat.Cursos)
                .WithOne(c => c.Categoria)
                .HasForeignKey(c => c.CategoriaId);

            // ── 9. Curso → Modulo (1 para Muitos) ────────────────────
            // Cascade: excluir o curso remove os modulos automaticamente.
            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Modulos)
                .WithOne(m => m.Curso)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── 10. Modulo → Aula (1 para Muitos) ────────────────────
            // Cascade: excluir o modulo remove as aulas automaticamente.
            modelBuilder.Entity<Modulo>()
                .HasMany(m => m.Aulas)
                .WithOne(a => a.Modulo)
                .HasForeignKey(a => a.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── 11. Matricula → Avaliacao (1 para Muitos) ────────────
            // Cascade: excluir a matricula remove as avaliacoes.
            modelBuilder.Entity<Matricula>()
                .HasMany(m => m.Avaliacoes)
                .WithOne(a => a.Matricula)
                .HasForeignKey(a => a.MatriculaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── 12. Matricula → Progresso (1 para 1) ─────────────────
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Progresso)
                .WithOne(p => p.Matricula)
                .HasForeignKey<Progresso>(p => p.MatriculaId);

            // ── 13. Matricula → Certificado (1 para 1) ───────────────
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Certificado)
                .WithOne(c => c.Matricula)
                .HasForeignKey<Certificado>(c => c.MatriculaId);

            // ── 14. AulaConcluida — PK composta (MatriculaId + AulaId) ──
            // Garante que a mesma aula so pode ser concluida uma vez por matricula.
            modelBuilder.Entity<AulaConcluida>()
                .HasKey(ac => new { ac.MatriculaId, ac.AulaId });

            // AulaConcluida → Matricula (Restrict: historico nao e apagado com a matricula)
            modelBuilder.Entity<AulaConcluida>()
                .HasOne(ac => ac.Matricula)
                .WithMany()
                .HasForeignKey(ac => ac.MatriculaId)
                .OnDelete(DeleteBehavior.Restrict);

            // AulaConcluida → Aula (Restrict: historico nao e apagado ao remover aula)
            modelBuilder.Entity<AulaConcluida>()
                .HasOne(ac => ac.Aula)
                .WithMany()
                .HasForeignKey(ac => ac.AulaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}