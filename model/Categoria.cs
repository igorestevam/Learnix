using System.Collections.Generic;

namespace Learnix.model
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        public List<Curso> Cursos { get; set; } = null!;

        public Categoria()
        {
            Cursos = new List<Curso>();
        }

        public Categoria(int id, string nome, string descricao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Cursos = new List<Curso>();
        }
    }
}
