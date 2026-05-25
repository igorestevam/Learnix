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
            // Substituído SQL raw por LINQ para respeitar o mapeamento TPH
            // (EF adiciona automaticamente o filtro de Discriminator)
            List<Curso> cursos = _context.Cursos
                .Where(c => c.Titulo.Contains(termoPesquisa))
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .ToList();

            return cursos;
        }
    }
}
