using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class AuthService : IAuthService
    {
        private readonly LearnixDbContext _context;

        public AuthService(LearnixDbContext context)
        {
            _context = context;
        }

        public Usuario RealizarLogin(string codigoAcesso, string senha)
        {
            // Busca por Aluno usando a matrícula académica
            Aluno alunoEncontrado = _context.Alunos
                .FirstOrDefault(a => a.MatriculaAcademica == codigoAcesso && a.Senha == senha);

            if (alunoEncontrado != null)
                return alunoEncontrado;

            // Busca por Instrutor usando Email como identificador universal
            // (Email já existe em Usuario, evitando dependência de Id numérico frágil)
            Instrutor instrutorEncontrado = _context.Instrutores
                .FirstOrDefault(i => i.Email == codigoAcesso && i.Senha == senha);

            if (instrutorEncontrado != null)
                return instrutorEncontrado;

            return null;
        }
    }
}
