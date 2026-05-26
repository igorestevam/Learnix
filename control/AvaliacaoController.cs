using System.Collections.Generic;
using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    /// <summary>
    /// Controller responsavel por orquestrar avaliacoes/notas.
    /// Usado pela TelaNotas.
    /// </summary>
    public class AvaliacaoController
    {
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        public Avaliacao? Registrar(int matriculaId, string titulo, double nota)
            => _avaliacaoService.RegistrarAvaliacao(matriculaId, titulo, nota);

        public List<Avaliacao> Listar(int matriculaId)
            => _avaliacaoService.ListarPorMatricula(matriculaId);

        public double Media(int matriculaId)
            => _avaliacaoService.CalcularMedia(matriculaId);

        public double MediaGeralDoAluno(int alunoId)
            => _avaliacaoService.CalcularMediaGeralDoAluno(alunoId);
    }
}
