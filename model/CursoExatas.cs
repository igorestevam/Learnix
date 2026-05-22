using System;

namespace Learnix.model
{
    public class CursoExatas : Curso
    {
        public bool PossuiLaboratorioVirtual { get; set; }
        public string FerramentaSoftwareSugerida { get; set; }

        public CursoExatas(int id, string titulo, int cargaHoraria, bool possuiLaboratorioVirtual)
            : base(id, titulo, cargaHoraria)
        {
            PossuiLaboratorioVirtual = possuiLaboratorioVirtual;
        }
    }
}