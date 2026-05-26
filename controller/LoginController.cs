using Learnix.Services;
using Learnix.model;

namespace Learnix.Controllers
{
    public class LoginController
    {
        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }

        public Usuario? RealizarLogin(string codigoAcesso, string senha)
        {
            return _authService.RealizarLogin(codigoAcesso, senha);
        }
    }
}
