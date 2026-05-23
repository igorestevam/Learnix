using System;

namespace Learnix.model
{
    /// <summary>
    /// Contrato para entidades que possuem avaliação com nota.
    /// Aplicado em: Avaliacao
    /// </summary>
    public interface IAvaliavel
    {
        int Id { get; set; }
        string Titulo { get; set; }
        double Nota { get; set; }
        DateTime DataRealizacao { get; set; }
    }
}
