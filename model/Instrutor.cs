using System;
using System.Collections.Generic;

namespace Learnix.model
{
    public class Instrutor : Usuario, IPlanejamento
    {
        public string Especialidade { get; set; } = null!;
        public string Biografia { get; set; } = null!;
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

        public void Definir()
        {
            if (string.IsNullOrWhiteSpace(Especialidade))
                throw new InvalidOperationException(
                    "Especialidade é obrigatória para definir o plano de ensino.");

            if (string.IsNullOrWhiteSpace(Biografia))
                Biografia = $"Instrutor especializado em {Especialidade}.";
        }
    }
}
