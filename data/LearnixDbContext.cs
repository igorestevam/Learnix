using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.data
{
    public class LearnixDbContext : DbContext
    {
        // ──────────────────────────────────────────────────────────────
        // DbSets — uma entrada por entidade concreta mapeada no banco
        // ──────────────────────────────────────────────────────────────

        // Hierarquia de Usuário (TPH → tabela única "Usuarios" + coluna discriminador)
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Instrutor> Instrutores { get; set; } = null!;

        public DbSet<PerfilDeAprendizagem> PerfisDeAprendizagem { get; set; } = null!;

        // Hierarquia de Curso (TPH → tabela única "Cursos" + coluna discriminador)
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

        // ──────────────────────────────────────────────────────────────
        // Construtores
        // ──────────────────────────────────────────────────────────────

        public LearnixDbContext() { }

        public LearnixDbContext(DbContextOptions<LearnixDbContext> options) : base(options) { }

        // ──────────────────────────────────────────────────────────────
        // Configuração da conexão (fallback quando não há DI configurada)
        // ──────────────────────────────────────────────────────────────

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=Learnix;Trusted_Connection=True;");
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Mapeamento do modelo
        // ──────────────────────────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── 1. TPH: hierarquia Usuario (Aluno / Instrutor) ────────
            // Tabela única "Usuarios"; coluna "TipoUsuario" diferencia os tipos.
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("TipoUsuario")
                .HasValue<Aluno>("Aluno")
                .HasValue<Instrutor>("Instrutor");

            // ── 2. TPH: hierarquia Curso (CursoExatas / CursoHumanas) ─
            // Tabela única "Cursos"; coluna "TipoCurso" diferencia os tipos.
            modelBuilder.Entity<Curso>()
                .HasDiscriminator<string>("TipoCurso")
                .HasValue<CursoExatas>("Exatas")
                .HasValue<CursoHumanas>("Humanas");

            // ── 3. Propriedade calculada — não persiste no banco ──────
            // Matricula.NotaFinal é calculada em memória; sem coluna no BD.
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
            // Restrict: impede excluir um aluno que tenha histórico de matrículas.
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
            // Cascade: excluir o curso remove os módulos automaticamente.
            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Modulos)
                .WithOne(m => m.Curso)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── 10. Modulo → Aula (1 para Muitos) ────────────────────
            // Cascade: excluir o módulo remove as aulas automaticamente.
            modelBuilder.Entity<Modulo>()
                .HasMany(m => m.Aulas)
                .WithOne(a => a.Modulo)
                .HasForeignKey(a => a.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── 11. Matricula → Avaliacao (1 para Muitos) ────────────
            // Cascade: excluir a matrícula remove as avaliações.
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
        }
    }
}