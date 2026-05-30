using System.Collections.Generic;

namespace Learnix.model
{
    public class Curso
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public int CargaHoraria { get; set; }
        public decimal Preco { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Nullable — curso pode existir sem instrutor vinculado
        public int? InstrutorId { get; set; }
        public Instrutor? Instrutor { get; set; }

        public List<Modulo> Modulos { get; set; } = null!;
        public List<Matricula> MatriculasAtivas { get; set; } = null!;
        public List<AtividadeCurso> Atividades { get; set; } = new List<AtividadeCurso>();

        public Curso()
        {
            Modulos = new List<Modulo>();
            MatriculasAtivas = new List<Matricula>();
        }

        public Curso(int id, string titulo, int cargaHoraria)
        {
            Id = id;
            Titulo = titulo;
            CargaHoraria = cargaHoraria;
            Modulos = new List<Modulo>();
            MatriculasAtivas = new List<Matricula>();
        }
    }
}