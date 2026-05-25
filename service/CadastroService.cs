using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Servico responsavel por persistir novos usuarios (Aluno/Instrutor) no banco.
    /// Contem regra de negocio de unicidade de e-mail.
    /// </summary>
    public class CadastroService : ICadastroService
    {
        private readonly LearnixDbContext _context;

        public CadastroService(LearnixDbContext context)
        {
            _context = context;
        }

        public bool EmailExiste(string email)
        {
            // Verifica em Alunos e Instrutores (mesma tabela TPH 'Usuarios')
            bool existeAluno = _context.Alunos.Any(a => a.Email == email);
            bool existeInstrutor = _context.Instrutores.Any(i => i.Email == email);
            return existeAluno || existeInstrutor;
        }

        public Aluno? CadastrarAluno(string nome, string email, string senha, string matriculaAcademica)
        {
            // Regra de Negocio: e-mail unico no sistema
            if (EmailExiste(email))
                return null;

            // Regra de Negocio: matricula academica unica
            bool matriculaJaExiste = _context.Alunos.Any(a => a.MatriculaAcademica == matriculaAcademica);
            if (matriculaJaExiste)
                return null;

            Aluno novoAluno = new Aluno
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                MatriculaAcademica = matriculaAcademica
            };

            _context.Alunos.Add(novoAluno);
            _context.SaveChanges();

            return novoAluno;
        }

        public Instrutor? CadastrarInstrutor(string nome, string email, string senha, string especialidade)
        {
            if (EmailExiste(email))
                return null;

            Instrutor novoInstrutor = new Instrutor
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                Especialidade = especialidade,
                Biografia = string.Empty
            };

            _context.Instrutores.Add(novoInstrutor);
            _context.SaveChanges();

            return novoInstrutor;
        }
    }
}
