using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.Repositorio
{
    public class CursoRepository : ICursoRepository
    {
        private readonly LearnixDbContext _context;

        // Injeção de dependência do contexto do banco
        public CursoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public List<Curso> BuscarCursosPorNome(string termoPesquisa)
        {
            string query = "SELECT * FROM Cursos WHERE Titulo LIKE {0}";

            // Tipagem explícita na lista
            List<Curso> cursos = _context.Cursos
                .FromSqlRaw(query, $"%{termoPesquisa}%")
                .ToList();

            return cursos;
        }
    }
}