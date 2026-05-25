using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.Repositorio
{
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly LearnixDbContext _context;

        public MatriculaRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            _context.SaveChanges();
        }

        public bool ExisteMatriculaAtiva(int alunoId, int cursoId)
        {
            // O repositorio encapsula a consulta, entregando apenas o booleano que a regra de negocio precisa
            bool existe = _context.Matriculas
                .Any(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Status == StatusMatricula.Ativa);

            return existe;
        }

        public Matricula? BuscarPorId(int id)
        {
            // Trazendo a matricula com todos os dados atrelados (Progresso, Cursos e Aulas) usando Include
            Matricula? matricula = _context.Matriculas
                .Include(m => m.Progresso)
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Categoria)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                        .ThenInclude(mod => mod.Aulas)
                .Include(m => m.Avaliacoes)
                .Include(m => m.Certificado)
                .FirstOrDefault(m => m.Id == id);

            return matricula;
        }

        public int ContarTotal()
        {
            int total = _context.Matriculas.Count();
            return total;
        }

        public List<Matricula> BuscarPorAluno(int alunoId)
        {
            // Carrega todas as matriculas de um aluno com os dados necessarios para
            // TelaMeusCursos, TelaNotas e TelaCertificados
            return _context.Matriculas
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Categoria)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                        .ThenInclude(mod => mod.Aulas)
                .Include(m => m.Progresso)
                .Include(m => m.Certificado)
                .Include(m => m.Avaliacoes)
                .Where(m => m.AlunoId == alunoId)
                .ToList();
        }
    }
}
