using System;
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

        public Matricula CriarMatricula(int alunoId, int cursoId)
        {
            Aluno aluno = _context.Alunos.FirstOrDefault(a => a.Id == alunoId);
            Curso curso = _context.Cursos.FirstOrDefault(c => c.Id == cursoId);

            if (aluno == null || curso == null)
            {
                return null;
            }

            // Regra de Negócio: Evita duplicidade de matrículas ativas no mesmo curso
            bool jaMatriculado = _context.Matriculas
                .Any(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Status == StatusMatricula.Ativa);

            if (jaMatriculado)
            {
                return null;
            }

            int proximoId = _context.Matriculas.Count() + 1;
            Matricula novaMatricula = new Matricula(proximoId, aluno, curso);

            // Cria e vincula o registro de progresso inicial
            Progresso novoProgresso = new Progresso(proximoId);
            novaMatricula.Progresso = novoProgresso;

            _context.Matriculas.Add(novaMatricula);
            _context.SaveChanges();

            return novaMatricula;
        }
    }
}