using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.repositorio
{
    public class CursoRepository
    {
        public void SalvarNoBanco(Curso curso)
        {
            using var context = new LearnixDbContext();

            // == Salvar no banco ==
            context.Cursos.Add(curso);
            context.SaveChanges();
        }

        public List<Curso> RecuperarTodosOsDados()
        {
            using var context = new LearnixDbContext();

            // Console.WriteLine("SELECT *");
            var dadosRecuperados = context.Cursos.ToList();

            return dadosRecuperados;
        }

        public Curso? RecuperarDadoEspecifico(int id)
        {
            using var context = new LearnixDbContext();

            // Console.WriteLine("SELECT WHERE V2.0");
            var dadoRecuperado = context.Cursos.FirstOrDefault(d => d.Id == id);

            return dadoRecuperado;
        }

        public void DeletarDado(int id)
        {
            using var context = new LearnixDbContext();
            var dadoRecuperado = context.Cursos.FirstOrDefault(d => d.Id == id);

            // Console.WriteLine("DELETE WHERE");
            if (dadoRecuperado != null)
            {
                context.Cursos.Remove(dadoRecuperado);
                context.SaveChanges();
            }
        }

        public List<Curso> BuscarCursosPorNomeSqlPuro(string termoPesquisa)
        {
            using var context = new LearnixDbContext();

            string query = "SELECT * FROM Cursos WHERE Titulo LIKE {0}";
            var dadosRecuperados = context.Cursos.FromSqlRaw(query, $"%{termoPesquisa}%").ToList();

            return dadosRecuperados;
        }
    }
}