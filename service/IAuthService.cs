using Learnix.model;

namespace Learnix.Services
{
    public interface IAuthService
    {
        Usuario? RealizarLogin(string codigoAcesso, string senha);
    }
}
