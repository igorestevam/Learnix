using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Representa o aluno da plataforma.
    /// Telas: TelaLogin (login por MatriculaAcademica), TelaPerfil (Nome, Email, MatriculaAcademica),
    ///        TelaHome (Nome), TelaMeusCursos (via Matricula)
    /// </summary>
    public class Aluno : Usuario
    {
        // Usado no login (AuthService) e exibido na TelaPerfil
        public string MatriculaAcademica { get; set; } = null!;

        // Exibido na TelaPerfil — estilo de aprendizagem do aluno (1 para 1)
        public int? PerfilDeAprendizagemId { get; set; }
        public PerfilDeAprendizagem? Perfil { get; set; }

        // Histórico completo de matrículas — base para TelaMeusCursos, TelaNotas e TelaCertificados
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

        public override string ObterCaminhoDashboard()
        {
            return $"/PainelAluno/Home?matricula={MatriculaAcademica}";
        }
    }
}
