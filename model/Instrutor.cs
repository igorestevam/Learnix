using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Representa o instrutor de cursos.
    /// Telas: TelaMenu (Nome), TelaMeusCursos (Nome), TelaAulas (Nome),
    ///        TelaCertificados (Nome), TelaHome (Nome)
    /// </summary>
    public class Instrutor : Usuario, IPlanejamento
    {
        // Exibido no TelaMenu e TelaMeusCursos como identificação do instrutor
        public string Especialidade { get; set; } = null!;

        // Reservado para tela de perfil do instrutor (expansão futura)
        public string Biografia { get; set; } = null!;

        // Cursos que este instrutor ministra (1 para muitos)
        public List<Curso> Cursos { get; set; } = null!;

        public Instrutor() : base()
        {
            Cursos = new List<Curso>();
        }

        public Instrutor(int id, string nome, string email, string especialidade)
            : base(id, nome, email)
        {
            Especialidade = especialidade;
            Cursos = new List<Curso>();
        }

        // IPlanejamento — permite ao instrutor definir o plano de ensino do curso
        public void Definir() { }

        public override string ObterCaminhoDashboard()
        {
            return $"/PainelInstrutor/Home?id={Id}";
        }
    }
}
