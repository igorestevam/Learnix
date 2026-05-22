using System.Collections.Generic;

namespace Learnix.model
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public List<Curso> Cursos { get; set; }

        // Construtor Vazio
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