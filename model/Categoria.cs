using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Categoria de um curso (ex: Exatas, Humanas, Tecnologia).
    /// Telas: TelaMenu (filtro por categoria), TelaMeusCursos (tag da categoria)
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }

        // Ex: "Exatas", "Humanas", "Tecnologia" — exibido nas tags das telas
        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        // Cursos que pertencem a esta categoria
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
