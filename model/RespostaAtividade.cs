namespace Learnix.model
{
    public class RespostaAtividade
    {
        public int Id { get; set; }
        public string Resposta { get; set; } = "";

        public decimal? Nota { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public int AtividadeCursoId { get; set; }
        public AtividadeCurso AtividadeCurso { get; set; } = null!;
    }
}