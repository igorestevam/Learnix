using Learnix.model;
using System.Collections.Generic;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Contrato de persistência para Avaliacao.
    /// Usado por: TelaNotas (listar AV1/AV2/AV3 e notas de uma matrícula).
    /// </summary>
    public interface IAvaliacaoRepository
    {
        /// <summary>Persiste uma nova avaliação no banco.</summary>
        void Adicionar(Avaliacao avaliacao);

        /// <summary>Atualiza a nota de uma avaliação existente.</summary>
        void Atualizar(Avaliacao avaliacao);

        /// <summary>Lista todas as avaliações de uma matrícula específica.</summary>
        List<Avaliacao> BuscarPorMatricula(int matriculaId);
    }
}
