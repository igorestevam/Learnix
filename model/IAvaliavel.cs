using System;

namespace Learnix.model
{
    public interface IAvaliavel
    {
        int Id { get; set; }
        string Titulo { get; set; }
        double Nota { get; set; }
        DateTime DataRealizacao { get; set; }
    }
}