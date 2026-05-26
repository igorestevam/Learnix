using System;

namespace Learnix.model
{
    /// <summary>
    /// Registra que um Aluno (via Matricula) concluiu uma Aula especifica.
    /// PK composta (MatriculaId + AulaId) garante que a mesma aula nao seja
    /// contada mais de uma vez no progresso — corrige o bug do ProgressoService
    /// que ignorava o aulaId e permitia incremento duplicado.
    /// Usada por: ProgressoService.RegistrarConclusaoAula
    /// </summary>
    public class AulaConcluida
    {
        // PK composta configurada no OnModelCreating do LearnixDbContext
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public int AulaId { get; set; }
        public Aula Aula { get; set; } = null!;

        // Data em que o aluno concluiu a aula — util para historico e relatorios
        public DateTime DataConclusao { get; set; }

        public AulaConcluida()
        {
            DataConclusao = DateTime.Now;
        }

        public AulaConcluida(int matriculaId, int aulaId)
        {
            MatriculaId = matriculaId;
            AulaId = aulaId;
            DataConclusao = DateTime.Now;
        }
    }
}
