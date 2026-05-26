using Learnix.model;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Contrato de persistência para Aluno.
    /// Usado por: AuthService (login), TelaPerfil (salvar dados), MatriculaService (buscar aluno).
    /// </summary>
    public interface IAlunoRepository
    {
        /// <summary>Busca aluno pelo número de matrícula acadêmica (usado no login).</summary>
        Aluno? BuscarPorMatricula(string matriculaAcademica);

        /// <summary>Busca aluno por e-mail (usado no cadastro para verificar duplicidade).</summary>
        Aluno? BuscarPorEmail(string email);

        /// <summary>Busca aluno pelo Id com todos os dados relacionados carregados.</summary>
        Aluno? BuscarPorId(int id);

        /// <summary>Persiste um novo aluno no banco.</summary>
        void Adicionar(Aluno aluno);

        /// <summary>Atualiza os dados de um aluno existente (perfil, senha, etc.).</summary>
        void Atualizar(Aluno aluno);
    }
}
