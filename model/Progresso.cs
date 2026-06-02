using System;

namespace Learnix.model
{
    public class Progresso
    {
        public int Id { get; set; }

        public double PercentualConcluido { get; set; }

        public DateTime UltimaAtualizacao { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public Progresso()
        {
            PercentualConcluido = 0.0;
            UltimaAtualizacao = DateTime.Now;
        }

        public Progresso(int id)
        {
            Id = id;
            PercentualConcluido = 0.0;
            UltimaAtualizacao = DateTime.Now;
        }
    }
}
