using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Implementacao de persistencia de Progresso usando Entity Framework.
    /// </summary>
    public class ProgressoRepository : IProgressoRepository
    {
        private readonly LearnixDbContext _context;

        public ProgressoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public Progresso? BuscarPorMatricula(int matriculaId)
        {
            return _context.Progressos
                .Include(p => p.Matricula)
                .FirstOrDefault(p => p.MatriculaId == matriculaId);
        }

        public void Atualizar(Progresso progresso)
        {
            _context.Progressos.Update(progresso);
            _context.SaveChanges();
        }

        public void Adicionar(Progresso progresso)
        {
            _context.Progressos.Add(progresso);
            _context.SaveChanges();
        }
    }
}
