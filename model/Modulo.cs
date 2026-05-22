using System.Collections.Generic;

namespace Learnix.model
{
    public class Modulo
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        public List<Aula> Aulas { get; set; }

        // Construtor Vazio
        public Modulo()
        {
            Aulas = new List<Aula>();
        }

        public Modulo(int id, string titulo, int ordem)
        {
            Id = id;
            Titulo = titulo;
            Ordem = ordem;
            Aulas = new List<Aula>();
        }
    }
}