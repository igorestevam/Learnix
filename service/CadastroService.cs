using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class CadastroService : ICadastroService
    {
        private readonly LearnixDbContext _context;

        public CadastroService(LearnixDbContext context)
        {
            _context = context;
        }

        public bool EmailExiste(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
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

            // Cria o aluno LIMPO - sem perfil, sem matriculas, sem historico
            // Cada dado (perfil, matriculas, progresso) sera adicionado conforme o aluno usa o sistema
            Aluno novoAluno = new Aluno
            {
                Nome                = nome,
                Email               = email,
                Senha               = senha,
                MatriculaAcademica  = matriculaAcademica,
                DataCadastro        = System.DateTime.Now,
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
                Nome          = nome,
                Email         = email,
                Senha         = senha,
                DataCadastro  = System.DateTime.Now,
                Especialidade = especialidade,
                Biografia     = string.Empty,
            };

            _context.Instrutores.Add(novoInstrutor);
            _context.SaveChanges();

            return novoInstrutor;
        }
    }
}
