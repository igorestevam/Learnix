namespace Learnix.model
{
    /// <summary>
    /// Perfil de aprendizagem vinculado a um Aluno (1 para 1).
    /// Telas: TelaPerfil (EstiloPredominante, RitmoSugerido)
    /// </summary>
    public class PerfilDeAprendizagem
    {
        public int Id { get; set; }

        // Ex: "Visual", "Auditivo", "Leitura/Escrita", "Cinestésico"
        public string EstiloPredominante { get; set; } = null!;

        // Ex: "Intensivo", "Regular", "Flexível"
        public string RitmoSugerido { get; set; } = null!;

        // Navegação inversa para o Aluno dono deste perfil
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
