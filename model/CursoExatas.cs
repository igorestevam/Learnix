namespace Learnix.model
{
    /// <summary>
    /// Curso da área de Exatas. Herda de Curso (necessário para TPH no EF).
    /// Telas: TelaMenu (tag "Exatas"), TelaMeusCursos (tag "Exatas")
    /// Propriedades específicas: diferenciam este tipo de curso no banco (TPH)
    /// </summary>
    public class CursoExatas : Curso
    {
        // Indica se o curso oferece laboratório virtual (ex: simuladores de circuito, IDEs online)
        public bool PossuiLaboratorioVirtual { get; set; }

        // Ferramenta sugerida para os exercícios (ex: "Visual Studio", "GeoGebra")
        public string FerramentaSoftwareSugerida { get; set; } = null!;

        public CursoExatas() : base() { }

        public CursoExatas(int id, string titulo, int cargaHoraria, bool possuiLaboratorioVirtual)
            : base(id, titulo, cargaHoraria)
        {
            PossuiLaboratorioVirtual = possuiLaboratorioVirtual;
        }
    }
}
