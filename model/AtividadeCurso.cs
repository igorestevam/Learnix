namespace Learnix.model
{
    public class AtividadeCurso
    {
        public int Id { get; set; }
        public string Pergunta { get; set; } = "";
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;
    }
}