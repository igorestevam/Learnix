using System;
using System.Collections.Generic;

namespace Learnix.model
{
    public class Matricula
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; } = null!;

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        public DateTime DataMatricula { get; set; }

        public StatusMatricula Status { get; set; }

        public Progresso Progresso { get; set; } = null!;

        public Certificado Certificado { get; set; } = null!;

        public List<Avaliacao> Avaliacoes { get; set; } = null!;

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
