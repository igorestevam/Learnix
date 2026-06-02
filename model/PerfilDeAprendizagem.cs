namespace Learnix.model
{
    public class PerfilDeAprendizagem
    {
        public int Id { get; set; }

        public string EstiloPredominante { get; set; } = null!;

        public string RitmoSugerido { get; set; } = null!;

        public Aluno Aluno { get; set; } = null!;

        public PerfilDeAprendizagem() { }

        public PerfilDeAprendizagem(int id, string estiloPredominante, string ritmoSugerido)
        {
            Id = id;
            EstiloPredominante = estiloPredominante;
            RitmoSugerido = ritmoSugerido;
        }
    }
}
