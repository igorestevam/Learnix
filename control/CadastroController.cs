using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    /// <summary>
    /// Controller responsavel por orquestrar o cadastro de novos usuarios.
    /// Chamado pela TelaCadastro.
    /// </summary>
    public class CadastroController
    {
        private readonly ICadastroService _cadastroService;

        public CadastroController(ICadastroService cadastroService)
        {
            _cadastroService = cadastroService;
        }

        /// <summary>
        /// Cadastra um Aluno. A matricula academica e gerada automaticamente a partir do email.
        /// Retorna o Aluno criado ou null em caso de e-mail/matricula duplicados.
        /// </summary>
        public Aluno? CadastrarAluno(string nome, string email, string senha)
        {
            // Gera uma matricula academica simples baseada no e-mail (parte antes do @)
            string baseEmail = email.Contains('@') ? email.Split('@')[0] : email;
            string matriculaAcademica = baseEmail.ToUpper();

            return _cadastroService.CadastrarAluno(nome, email, senha, matriculaAcademica);
        }

        /// <summary>
        /// Cadastra um Instrutor.
        /// </summary>
        public Instrutor? CadastrarInstrutor(string nome, string email, string senha, string especialidade)
        {
            return _cadastroService.CadastrarInstrutor(nome, email, senha, especialidade);
        }
    }
}
