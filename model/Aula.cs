using System;

namespace Learnix.model
{
    public class Aula
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string VideoUrl { get; set; }
        public TimeSpan Duracao { get; set; }
        public int Ordem { get; set; }

        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }

        public Aula(int id, string titulo, string videoUrl, TimeSpan duracao, int ordem)
        {
            Id = id;
            Titulo = titulo;
            VideoUrl = videoUrl;
            Duracao = duracao;
            Ordem = ordem;
        }
    }
}