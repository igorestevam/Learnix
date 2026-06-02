using System;

namespace Learnix.model
{
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public DateTime DataCadastro { get; set; }

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

    }
}
