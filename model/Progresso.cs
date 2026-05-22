using System;

namespace Learnix.model
{
    public class Progresso
    {
        public int Id { get; set; }
        public int AulasConcluidas { get; set; }
        public double PercentualConcluido { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; }

        public Progresso(int id)
        {
            Id = id;
            AulasConcluidas = 0;
            PercentualConcluido = 0.0;
            UltimaAtualizacao = DateTime.Now;
        }
    }
}