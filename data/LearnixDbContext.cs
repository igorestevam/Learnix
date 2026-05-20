using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learnix.data
{
    internal class LearnixDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Instrutor> Instrutores { get; set; } = null!;
        public DbSet<PerfilDeAprendizagem> PerfisDeAprendizagem { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<CursoExatas> CursosExatas { get; set; } = null!;
        public DbSet<CursoHumanas> CursosHumanas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Perfil)
                .WithOne(p => p.Aluno)
                .HasForeignKey<Aluno>(a => a.PerfilDeAprendizagemId);

            modelBuilder.Entity<Instrutor>()
                .HasMany(i => i.Cursos)
                .WithOne(c => c.Instrutor)
                .HasForeignKey(c => c.InstrutorId);
        }
    }
}
