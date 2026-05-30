namespace Learnix.model
{
    /// <summary>
    /// Status de uma matrícula.
    /// Telas: TelaMeusCursos (Ativa → botão "Acessar", Concluida → botão "Concluir" desabilitado)
    /// </summary>
    public enum StatusMatricula
    {
        Ativa,
        AguardandoContinuar,
        AguardandoCorrecao,
        Concluida,
        Cancelada
    }
}
