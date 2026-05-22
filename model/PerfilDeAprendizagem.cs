namespace Learnix.model
{
    public class PerfilDeAprendizagem
    {
        public int Id { get; set; }
        public string EstiloPredominante { get; set; }
        public string RitmoSugerido { get; set; }

        public Aluno Aluno { get; set; }

        public PerfilDeAprendizagem(int id, string estiloPredominante)
        {
            Id = id;
            EstiloPredominante = estiloPredominante;
        }
    }
}