using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.Repositorio
{
    public class CursoRepository
    {
        public List<Curso> BuscarCursosPorNome(string termoPesquisa)
        {
            using var context = new LearnixDbContext();

            string query = "SELECT * FROM Cursos WHERE Titulo LIKE {0}";

            var cursos = context.Cursos
                .FromSqlRaw(query, $"%{termoPesquisa}%")
                .ToList();

            return cursos;
        }
    }
}