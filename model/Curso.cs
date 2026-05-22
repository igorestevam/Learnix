using System;
using System.Collections.Generic;

namespace Learnix.model
{
    public abstract class Curso
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int CargaHoraria { get; set; }
        public decimal Preco { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public int InstrutorId { get; set; }
        public Instrutor Instrutor { get; set; }

        public List<Modulo> Modulos { get; set; }
        public List<Matricula> MatriculasAtivas { get; set; }

        protected Curso(int id, string titulo, int cargaHoraria)
        {
            Id = id;
            Titulo = titulo;
            CargaHoraria = cargaHoraria;
            Modulos = new List<Modulo>();
            MatriculasAtivas = new List<Matricula>();
        }
    }
}