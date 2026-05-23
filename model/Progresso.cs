using System;

namespace Learnix.model
{
    /// <summary>
    /// Rastreia o avanço de um Aluno dentro de uma Matricula (1 para 1).
    /// Telas: TelaHome (barra de progresso e %), TelaMeusCursos (barra de progresso),
    ///        TelaAulas (% exibido no círculo de progresso)
    /// </summary>
    public class Progresso
    {
        public int Id { get; set; }

        // Quantidade de aulas assistidas — usada no ProgressoService para calcular %
        public int AulasConcluidas { get; set; }

        // Percentual exibido nas barras de progresso das telas (0.0 a 100.0)
        public double PercentualConcluido { get; set; }

        // Controle interno — atualizado sempre que uma aula é concluída
        public DateTime UltimaAtualizacao { get; set; }

        // Matrícula à qual este progresso pertence
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public Progresso()
        {
            AulasConcluidas = 0;
            PercentualConcluido = 0.0;
            UltimaAtualizacao = DateTime.Now;
        }

        public Progresso(int id)
        {
            Id = id;
            AulasConcluidas = 0;
            PercentualConcluido = 0.0;
            UltimaAtualizacao = DateTime.Now;
        }
    }
}
