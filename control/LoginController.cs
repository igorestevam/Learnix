using System;
using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    /// <summary>
    /// Controller responsavel por orquestrar o login.
    /// Chamado pela TelaLogin.
    /// </summary>
    public class LoginController
    {
        private readonly IAuthService _authService;

        // Injecao de Dependencia via contrato (Interface)
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica o usuario e retorna o objeto Usuario (Aluno ou Instrutor)
        /// para a camada de View propagar adiante. Retorna null se credenciais invalidas.
        /// Este e o metodo usado pela TelaLogin.
        /// </summary>
        public Usuario? AutenticarUsuario(string inputUnico, string senhaInserida)
        {
            return _authService.RealizarLogin(inputUnico, senhaInserida);
        }

        /// <summary>
        /// Mantido por compatibilidade. Apenas loga o resultado no console.
        /// Prefira AutenticarUsuario(...) que retorna o objeto Usuario.
        /// </summary>
        public void ProcessarLogin(string inputUnico, string senhaInserida)
        {
            Usuario? usuarioAutenticado = _authService.RealizarLogin(inputUnico, senhaInserida);

            if (usuarioAutenticado == null)
            {
                Console.WriteLine("Erro: Codigo de acesso ou senha invalidos.");
                return;
            }

            // O Polimorfismo decide o caminho correto sem ifs redundantes
            string caminhoRedirecionamento = usuarioAutenticado.ObterCaminhoDashboard();

            Console.WriteLine($"Redirecionando sistema para a rota: {caminhoRedirecionamento}");
        }
    }
}