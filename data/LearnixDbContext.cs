using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.data
{
    public class LearnixDbContext : DbContext
    {
        // Declaração explícita de todas as tabelas do sistema
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Instrutor> Instrutores { get; set; } = null!;
        public DbSet<PerfilDeAprendizagem> PerfisDeAprendizagem { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<CursoExatas> CursosExatas { get; set; } = null!;
        public DbSet<CursoHumanas> CursosHumanas { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Modulo> Modulos { get; set; } = null!;
        public DbSet<Aula> Aulas { get; set; } = null!;
        public DbSet<Avaliacao> Avaliacoes { get; set; } = null!;
        public DbSet<Progresso> Progressos { get; set; } = null!;
        public DbSet<Certificado> Certificados { get; set; } = null!;

        public LearnixDbContext()
        {
        }

        public LearnixDbContext(DbContextOptions<LearnixDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // String de conexão apontando para o LocalDB do SQL Server
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Relação Aluno -> PerfilDeAprendizagem (1 para 1)
            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Perfil)
                .WithOne(p => p.Aluno)
                .HasForeignKey<Aluno>(a => a.PerfilDeAprendizagemId);

            // 2. Relação Instrutor -> Curso (1 para Muitos)
            modelBuilder.Entity<Instrutor>()
                .HasMany(i => i.Cursos)
                .WithOne(c => c.Instrutor)
                .HasForeignKey(c => c.InstrutorId);

            // 3. Relação Aluno -> Matricula (1 para Muitos)
            // Restrict: Impede apagar um aluno que tenha matrículas ativas no histórico
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Aluno)
                .WithMany(a => a.HistoricoMatriculas)
                .HasForeignKey(m => m.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Relação Curso -> Matricula (1 para Muitos)
            // Restrict: Impede apagar um curso que tenha alunos matriculados nele
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Curso)
                .WithMany(c => c.MatriculasAtivas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Relação Categoria -> Curso (1 para Muitos)
            modelBuilder.Entity<Categoria>()
                .HasMany(cat => cat.Cursos)
                .WithOne(c => c.Categoria)
                .HasForeignKey(c => c.CategoriaId);

            // 6. Relação Curso -> Modulo (1 para Muitos)
            // Cascade: Se o curso for apagado, os módulos somem junto
            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Modulos)
                .WithOne(m => m.Curso)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. Relação Modulo -> Aula (1 para Muitos)
            // Cascade: Se o módulo for apagado, as aulas dele somem junto
            modelBuilder.Entity<Modulo>()
                .HasMany(m => m.Aulas)
                .WithOne(a => a.Modulo)
                .HasForeignKey(a => a.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // 8. Relação Matricula -> Avaliacao (1 para Muitos)
            modelBuilder.Entity<Matricula>()
                .HasMany(m => m.Avaliacoes)
                .WithOne(a => a.Matricula)
                .HasForeignKey(a => a.MatriculaId)
                .OnDelete(DeleteBehavior.Cascade);

            // 9. Relação Matricula -> Progresso (1 para 1)
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Progresso)
                .WithOne(p => p.Matricula)
                .HasForeignKey<Progresso>(p => p.MatriculaId);

            // 10. Relação Matricula -> Certificado (1 para 1)
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Certificado)
                .WithOne(c => c.Matricula)
                .HasForeignKey<Certificado>(c => c.MatriculaId);
        }
    }
}