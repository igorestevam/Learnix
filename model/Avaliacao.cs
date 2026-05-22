using System;

namespace Learnix.model
{
    public class Avaliacao : IAvaliavel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public double Nota { get; set; }
        public DateTime DataRealizacao { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; }

        // Construtor Vazio
        public Avaliacao()
        {
            DataRealizacao = DateTime.Now;
        }

        public Avaliacao(int id, string titulo, double nota)
        {
            Id = id;
            Titulo = titulo;
            Nota = nota;
            DataRealizacao = DateTime.Now;
        }
    }
}