using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Contrato do servico responsavel pelo cadastro de novos usuarios (Aluno e Instrutor).
    /// Usado por: TelaCadastro -> CadastroController -> CadastroService.
    /// </summary>
    public interface ICadastroService
    {
        /// <summary>
        /// Cadastra um novo Aluno no banco. Retorna o Aluno persistido ou null se o e-mail ja existir.
        /// </summary>
        Aluno? CadastrarAluno(string nome, string email, string senha, string matriculaAcademica);

        /// <summary>
        /// Cadastra um novo Instrutor no banco. Retorna o Instrutor persistido ou null se o e-mail ja existir.
        /// </summary>
        Instrutor? CadastrarInstrutor(string nome, string email, string senha, string especialidade);

        /// <summary>
        /// Verifica se um e-mail ja esta cadastrado (em Aluno ou Instrutor).
        /// </summary>
        bool EmailExiste(string email);
    }
}
