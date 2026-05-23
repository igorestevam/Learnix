using System;

namespace Learnix.model
{
    /// <summary>
    /// Certificado emitido automaticamente quando o Progresso atinge 100%.
    /// Telas: TelaCertificados (CodigoCertificado, DataEmissao,
    ///        via Matricula → Aluno.Nome, Curso.Titulo, Instrutor.Nome, Curso.CargaHoraria)
    /// Gerado pelo: ProgressoService.RegistrarConclusaoAula (quando percentual >= 100)
    /// </summary>
    public class Certificado
    {
        public int Id { get; set; }

        // Data de emissão exibida no certificado visual
        public DateTime DataEmissao { get; set; }

        // Código único exibido no rodapé do certificado (ex: "LX-A3F9B2")
        public string CodigoCertificado { get; set; } = null!;

        // Matrícula que originou este certificado (1 para 1)
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public Certificado()
        {
            DataEmissao = DateTime.Now;
        }

        public Certificado(int id, string codigoCertificado)
        {
            Id = id;
            CodigoCertificado = codigoCertificado;
            DataEmissao = DateTime.Now;
        }
    }
}
