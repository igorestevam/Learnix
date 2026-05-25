using Learnix.data;
using Learnix.model;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Implementação de persistência para Avaliacao usando Entity Framework + LocalDB.
    /// </summary>
    public class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly LearnixDbContext _context;

        public AvaliacaoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Avaliacao avaliacao)
        {
            _context.Avaliacoes.Add(avaliacao);
            _context.SaveChanges();
        }

        public void Atualizar(Avaliacao avaliacao)
        {
            _context.Avaliacoes.Update(avaliacao);
            _context.SaveChanges();
        }

        public List<Avaliacao> BuscarPorMatricula(int matriculaId)
        {
            return _context.Avaliacoes
                .Where(a => a.MatriculaId == matriculaId)
                .OrderBy(a => a.Titulo)
                .ToList();
        }
    }
}
