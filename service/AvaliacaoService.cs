using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Servico de Avaliacao. Implementa logica de registro e calculo de medias.
    /// </summary>
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly LearnixDbContext _context;

        public AvaliacaoService(LearnixDbContext context)
        {
            _context = context;
        }

        public Avaliacao? RegistrarAvaliacao(int matriculaId, string titulo, double nota)
        {
            Matricula? matricula = _context.Matriculas.FirstOrDefault(m => m.Id == matriculaId);
            if (matricula == null) return null;

            // Regra: nota entre 0 e 10
            if (nota < 0) nota = 0;
            if (nota > 10) nota = 10;

            Avaliacao av = new Avaliacao
            {
                Titulo = titulo,
                Nota = nota,
                MatriculaId = matriculaId
            };

            _context.Avaliacoes.Add(av);
            _context.SaveChanges();

            return av;
        }

        public List<Avaliacao> ListarPorMatricula(int matriculaId)
        {
            return _context.Avaliacoes
                .Where(a => a.MatriculaId == matriculaId)
                .OrderBy(a => a.DataRealizacao)
                .ToList();
        }

        public double CalcularMedia(int matriculaId)
        {
            var avaliacoes = _context.Avaliacoes
                .Where(a => a.MatriculaId == matriculaId)
                .ToList();

            if (avaliacoes.Count == 0) return 0.0;

            return avaliacoes.Average(a => a.Nota);
        }

        public double CalcularMediaGeralDoAluno(int alunoId)
        {
            var avaliacoes = _context.Avaliacoes
                .Include(a => a.Matricula)
                .Where(a => a.Matricula.AlunoId == alunoId)
                .ToList();

            if (avaliacoes.Count == 0) return 0.0;

            return avaliacoes.Average(a => a.Nota);
        }
    }
}
