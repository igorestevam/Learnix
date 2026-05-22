using System;

namespace Learnix.model
{
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public DateTime DataCadastro { get; set; }

        // Construtor Vazio (protected por ser uma classe abstrata)
        protected Usuario()
        {
            DataCadastro = DateTime.Now;
        }

        protected Usuario(int id, string nome, string email)
        {
            Id = id;
            Nome = nome;
            Email = email;
            DataCadastro = DateTime.Now;
        }

        public abstract string ObterCaminhoDashboard();
    }
}