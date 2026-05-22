using System;
using System.Collections.Generic;

namespace Learnix.model
{
    public class Matricula
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        public DateTime DataMatricula { get; set; }
        public StatusMatricula Status { get; set; }

        public Progresso Progresso { get; set; }
        public Certificado Certificado { get; set; }
        public List<Avaliacao> Avaliacoes { get; set; }

        public double NotaFinal
        {
            get
            {
                if (Avaliacoes == null || Avaliacoes.Count == 0)
                {
                    return 0.0;
                }

                double soma = 0.0;
                foreach (Avaliacao avaliacao in Avaliacoes)
                {
                    soma += avaliacao.Nota;
                }

                return soma / Avaliacoes.Count;
            }
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