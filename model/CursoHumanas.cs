namespace Learnix.model
{
    /// <summary>
    /// Curso da área de Humanas. Herda de Curso (necessário para TPH no EF).
    /// Telas: TelaMenu (tag "Humanas"), TelaMeusCursos (tag "Humanas")
    /// </summary>
    public class CursoHumanas : Curso
    {
        // Indica se o curso exige monografia ao final
        public bool ExigeMonografia { get; set; }

        // Número de livros obrigatórios na bibliografia do curso
        public int QuantidadeLivrosObrigatorios { get; set; }

        public CursoHumanas() : base() { }

        public CursoHumanas(int id, string titulo, int cargaHoraria, bool exigeMonografia)
            : base(id, titulo, cargaHoraria)
        {
            ExigeMonografia = exigeMonografia;
        }
    }
}
