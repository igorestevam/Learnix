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

        public Usuario? RealizarLogin(string codigoAcesso, string senha)
        {
            // Busca por Aluno usando matricula academica OU e-mail (mais flexivel)
            Aluno? alunoEncontrado = _context.Alunos
                .FirstOrDefault(a =>
                    (a.MatriculaAcademica == codigoAcesso || a.Email == codigoAcesso)
                    && a.Senha == senha);

            if (alunoEncontrado != null)
                return alunoEncontrado;

            // Busca por Instrutor usando Email como identificador universal
            Instrutor? instrutorEncontrado = _context.Instrutores
                .FirstOrDefault(i => i.Email == codigoAcesso && i.Senha == senha);

            if (instrutorEncontrado != null)
                return instrutorEncontrado;

            return null;
        }
    }
}
