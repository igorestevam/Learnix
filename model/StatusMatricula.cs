namespace Learnix.model
{
    /// <summary>
    /// Status de uma matrícula.
    /// Telas: TelaMeusCursos (Ativa → botão "Acessar", Concluida → botão "Concluir" desabilitado)
    /// </summary>
    public enum StatusMatricula
    {
        // Aluno está cursando — exibe botão "Acessar" na TelaMeusCursos
        Ativa,

        // Progresso = 100% — exibe botão "Concluir" e gera certificado
        Concluida,

        // Matrícula cancelada pelo aluno ou pela plataforma
        Cancelada,

        // Matrícula pausada temporariamente
        Pausada
    }
}
