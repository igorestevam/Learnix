using System;

namespace Learnix.model
{
    /// <summary>
    /// Representa uma aula dentro de um módulo.
    /// Telas: TelaAulas (Titulo, Duracao, Ordem — lista de aulas com status),
    ///        TelaPlayer (Titulo, VideoUrl — carregado no MediaElement)
    /// </summary>
    public class Aula
    {
        public int Id { get; set; }

        // Exibido na TelaAulas e na lista lateral do TelaPlayer
        public string Titulo { get; set; } = null!;

        // Caminho do arquivo de vídeo — carregado no MediaElement do TelaPlayer
        public string VideoUrl { get; set; } = null!;

        // Duração exibida na TelaAulas (ex: "22 min")
        public TimeSpan Duracao { get; set; } = default;

        // Define a ordem de exibição dentro do módulo
        public int Ordem { get; set; }

        // Módulo ao qual esta aula pertence
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; } = null!;

        public Aula() { }

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
