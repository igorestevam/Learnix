using System;

namespace Learnix.model
{
    public class Certificado
    {
        public int Id { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CodigoCertificado { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; }

        public Certificado(int id, string codigoCertificado)
        {
            Id = id;
            CodigoCertificado = codigoCertificado;
            DataEmissao = DateTime.Now;
        }
    }
}