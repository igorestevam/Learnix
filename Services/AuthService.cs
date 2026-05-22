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
            {
                return alunoEncontrado;
            }

            // Caso o código seja numérico, tenta buscar por Instrutor usando o ID
            int idInstrutor;
            bool isNumero = int.TryParse(codigoAcesso, out idInstrutor);

            if (isNumero)
            {
                Instrutor instrutorEncontrado = _context.Instrutores
                    .FirstOrDefault(i => i.Id == idInstrutor && i.Senha == senha);

                if (instrutorEncontrado != null)
                {
                    return instrutorEncontrado;
                }
            }

            return null;
        }
    }
}