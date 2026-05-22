using System;
using System.Collections.Generic;

namespace Learnix.model
{
    public class Instrutor : Usuario, IPlanejamento
    {
        public string Especialidade { get; set; }
        public string Biografia { get; set; }

        public List<Curso> Cursos { get; set; }

        // Construtor Vazio
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

        public void Definir()
        {
            // Lógica de planejamento do instrutor
        }

        public override string ObterCaminhoDashboard()
        {
            return $"/PainelInstrutor/Home?id={Id}";
        }
    }
}