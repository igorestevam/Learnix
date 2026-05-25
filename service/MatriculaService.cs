using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly LearnixDbContext _context;

        public MatriculaService(LearnixDbContext context)
        {
            _context = context;
        }

        public Matricula? CriarMatricula(int alunoId, int cursoId)
        {
            Aluno? aluno = _context.Alunos.FirstOrDefault(a => a.Id == alunoId);
            Curso? curso = _context.Cursos.FirstOrDefault(c => c.Id == cursoId);

            if (aluno == null || curso == null)
            {
                return null;
            }

            // Regra de Negocio: Evita duplicidade de matriculas ativas no mesmo curso
            bool jaMatriculado = _context.Matriculas
                .Any(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Status == StatusMatricula.Ativa);

            if (jaMatriculado)
            {
                return null;
            }

            // Deixa o EF gerar o Id automaticamente (Identity)
            Matricula novaMatricula = new Matricula
            {
                AlunoId = alunoId,
                CursoId = cursoId,
                Status = StatusMatricula.Ativa
            };

            _context.Matriculas.Add(novaMatricula);
            _context.SaveChanges();

            // Cria e vincula o registro de progresso inicial (apos ter o Id da matricula)
            Progresso novoProgresso = new Progresso
            {
                MatriculaId = novaMatricula.Id,
                AulasConcluidas = 0,
                PercentualConcluido = 0.0
            };

            _context.Progressos.Add(novoProgresso);
            _context.SaveChanges();

            return novaMatricula;
        }
    }
}
