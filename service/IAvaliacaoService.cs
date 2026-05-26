using System.Collections.Generic;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Contrato do servico de Avaliacao. Usado pela TelaNotas.
    /// </summary>
    public interface IAvaliacaoService
    {
        /// <summary>Registra uma nova avaliacao para uma matricula.</summary>
        Avaliacao? RegistrarAvaliacao(int matriculaId, string titulo, double nota);

        /// <summary>Lista todas as avaliacoes de uma matricula.</summary>
        List<Avaliacao> ListarPorMatricula(int matriculaId);

        /// <summary>Calcula a media das avaliacoes de uma matricula.</summary>
        double CalcularMedia(int matriculaId);

        /// <summary>Calcula a media geral de todas as matriculas de um aluno.</summary>
        double CalcularMediaGeralDoAluno(int alunoId);
    }
}
