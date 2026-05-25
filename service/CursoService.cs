using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Servico de Curso. Encapsula consultas usadas pela TelaMenu.
    /// </summary>
    public class CursoService : ICursoService
    {
        private readonly LearnixDbContext _context;

        public CursoService(LearnixDbContext context)
        {
            _context = context;
        }

        public List<Curso> ListarTodos()
        {
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .ToList();
        }

        public List<Curso> ListarPorCategoria(string nomeCategoria)
        {
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Where(c => c.Categoria.Nome == nomeCategoria)
                .ToList();
        }

        public List<Curso> BuscarPorTermo(string termo)
        {
            string termoLower = (termo ?? string.Empty).ToLower();
            return _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Where(c => c.Titulo.ToLower().Contains(termoLower))
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
    }
}
