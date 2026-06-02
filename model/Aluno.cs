using System.Collections.Generic;

namespace Learnix.model
{
    public class Aluno : Usuario
    {
        public string MatriculaAcademica { get; set; } = null!;

        public int? PerfilDeAprendizagemId { get; set; }
        public PerfilDeAprendizagem? Perfil { get; set; }
        public List<Matricula> HistoricoMatriculas { get; set; } = null!;

        public Aluno() : base()
        {
            HistoricoMatriculas = new List<Matricula>();
        }

        public Aluno(int id, string nome, string email, string matriculaAcademica)
            : base(id, nome, email)
        {
            MatriculaAcademica = matriculaAcademica;
            HistoricoMatriculas = new List<Matricula>();
        }

    }
}
