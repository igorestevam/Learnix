using System;
using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    public class LoginController
    {
        private readonly IAuthService _authService;

        // Injeção de Dependência via contrato (Interface)
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        public void ProcessarLogin(string inputUnico, string senhaInserida)
        {
            Usuario usuarioAutenticado = _authService.RealizarLogin(inputUnico, senhaInserida);

            if (usuarioAutenticado == null)
            {
                Console.WriteLine("Erro: Código de acesso ou senha inválidos.");
                return;
            }

            // O Polimorfismo decide o caminho correto sem ifs redundantes
            string caminhoRedirecionamento = usuarioAutenticado.ObterCaminhoDashboard();

            Console.WriteLine($"Redirecionando sistema para a rota: {caminhoRedirecionamento}");
        }
    }
}