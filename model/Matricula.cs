using System;
using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Vínculo entre Aluno e Curso. É o ponto central do sistema.
    /// Telas: TelaMeusCursos (DataMatricula, Status, Progresso),
    ///        TelaNotas (Avaliacoes, NotaFinal),
    ///        TelaCertificados (via Certificado),
    ///        TelaHome (Progresso, Status)
    /// </summary>
    public class Matricula
    {
        public int Id { get; set; }

        // Aluno matriculado
        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; } = null!;

        // Curso em que o aluno está matriculado
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        // Exibida na TelaMeusCursos como data de início
        public DateTime DataMatricula { get; set; }

        // Ativa, Concluida, Cancelada ou Pausada — controla o botão "Concluir" na TelaMeusCursos
        public StatusMatricula Status { get; set; }

        // Barra de progresso na TelaHome e TelaMeusCursos
        public Progresso Progresso { get; set; } = null!;

        // Emitido quando Progresso.PercentualConcluido >= 100 — exibido na TelaCertificados
        public Certificado Certificado { get; set; } = null!;

        // AV1, AV2, AV3 — exibidas na TelaNotas
        public List<Avaliacao> Avaliacoes { get; set; } = null!;

        /// <summary>
        /// Média aritmética das avaliações — exibida na TelaNotas como "Média"
        /// e no card de resumo da TelaHome
        /// </summary>
        public double NotaFinal
        {
            get
            {
                if (Avaliacoes == null || Avaliacoes.Count == 0) return 0.0;
                double soma = 0.0;
                foreach (var av in Avaliacoes) soma += av.Nota;
                return soma / Avaliacoes.Count;
            }
        }

        public Matricula()
        {
            DataMatricula = DateTime.Now;
            Status = StatusMatricula.Ativa;
            Avaliacoes = new List<Avaliacao>();
        }

        public Matricula(int id, Aluno aluno, Curso curso)
        {
            Id = id;
            Aluno = aluno;
            Curso = curso;
            DataMatricula = DateTime.Now;
            Status = StatusMatricula.Ativa;
            Avaliacoes = new List<Avaliacao>();
        }
    }
}
