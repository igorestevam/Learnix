using System;

namespace Learnix.model
{
    public class CursoHumanas : Curso
    {
        public bool ExigeMonografia { get; set; }
        public int QuantidadeLivrosObrigatorios { get; set; }

        // Construtor Vazio
        public CursoHumanas() : base()
        {
        }

        public CursoHumanas(int id, string titulo, int cargaHoraria, bool exigeMonografia)
            : base(id, titulo, cargaHoraria)
        {
            ExigeMonografia = exigeMonografia;
        }
    }
}