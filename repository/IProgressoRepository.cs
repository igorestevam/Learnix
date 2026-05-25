using Learnix.model;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Contrato de persistência para Progresso.
    /// Usado por: ProgressoService (registrar conclusão de aula, calcular percentual).
    /// </summary>
    public interface IProgressoRepository
    {
        /// <summary>Busca o progresso de uma matrícula específica.</summary>
        Progresso? BuscarPorMatricula(int matriculaId);

        /// <summary>Persiste um novo progresso (criado junto com a matrícula).</summary>
        void Adicionar(Progresso progresso);

        /// <summary>Atualiza o progresso existente (aulas concluídas, percentual).</summary>
        void Atualizar(Progresso progresso);
    }
}
