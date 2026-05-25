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

        public CursoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public List<Curso> BuscarTodos()
        {
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Aulas)
                .ToList();
        }

        public Curso? BuscarPorId(int id)
        {
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == id);
        }

        public List<Curso> BuscarCursosPorNome(string termoPesquisa)
        {
            // LINQ substitui SQL raw para respeitar o mapeamento TPH do EF
            return _context.Cursos
                .Where(c => c.Titulo.Contains(termoPesquisa))
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .ToList();
        }

        public List<Curso> BuscarPorCategoria(string nomeCategoria)
        {
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Where(c => c.Categoria != null && c.Categoria.Nome == nomeCategoria)
                .ToList();
        }
    }
}
