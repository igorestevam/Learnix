using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Agrupa aulas de um curso em módulos temáticos.
    /// Telas: TelaAulas (Titulo, Ordem — exibido como cabeçalho de seção),
    ///        TelaPlayer (lista lateral de aulas agrupada por módulo)
    /// </summary>
    public class Modulo
    {
        public int Id { get; set; }

        // Ex: "Módulo 1 — Introdução", exibido como cabeçalho na TelaAulas
        public string Titulo { get; set; } = null!;

        // Define a sequência de exibição dos módulos
        public int Ordem { get; set; }

        // Curso ao qual este módulo pertence
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        // Aulas deste módulo — exibidas na TelaAulas e lista lateral do TelaPlayer
        public List<Aula> Aulas { get; set; } = null!;

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
