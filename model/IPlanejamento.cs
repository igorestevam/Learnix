namespace Learnix.model
{
    /// <summary>
    /// Contrato para entidades que podem definir um plano de ensino.
    /// Aplicado em: Instrutor — garante especialidade definida e biografia configurada
    /// antes de o instrutor ministrar cursos.
    /// </summary>
    public interface IPlanejamento
    {
        void Definir();
    }
}
