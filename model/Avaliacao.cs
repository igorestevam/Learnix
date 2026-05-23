using System;

namespace Learnix.model
{
    /// <summary>
    /// Avaliação de um Aluno dentro de uma Matricula (AV1, AV2, AV3).
    /// Telas: TelaNotas (Titulo como "AV1"/"AV2"/"AV3", Nota, DataRealizacao)
    ///        TelaHome (contribui para o card "Média Geral" via Matricula.NotaFinal)
    /// </summary>
    public class Avaliacao : IAvaliavel
    {
        public int Id { get; set; }

        // Ex: "AV1", "AV2", "AV3" — exibido como cabeçalho da coluna na TelaNotas
        public string Titulo { get; set; } = null!;

        // Nota de 0 a 10 — exibida na tabela da TelaNotas
        public double Nota { get; set; }

        // Data em que a avaliação foi realizada
        public DateTime DataRealizacao { get; set; }

        // Matrícula a que esta avaliação pertence
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public Avaliacao()
        {
            DataRealizacao = DateTime.Now;
        }

        public Avaliacao(int id, string titulo, double nota)
        {
            Id = id;
            Titulo = titulo;
            Nota = nota;
            DataRealizacao = DateTime.Now;
        }
    }
}
