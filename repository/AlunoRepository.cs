using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Implementação de persistência para Aluno usando Entity Framework + LocalDB.
    /// </summary>
    public class AlunoRepository : IAlunoRepository
    {
        private readonly LearnixDbContext _context;

        public AlunoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public Aluno? BuscarPorMatricula(string matriculaAcademica)
        {
            return _context.Alunos
                .Include(a => a.Perfil)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Instrutor)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Categoria)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Progresso)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Certificado)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Avaliacoes)
                .FirstOrDefault(a => a.MatriculaAcademica == matriculaAcademica);
        }

        public Aluno? BuscarPorEmail(string email)
        {
            return _context.Alunos
                .FirstOrDefault(a => a.Email == email);
        }

        public Aluno? BuscarPorId(int id)
        {
            return _context.Alunos
                .Include(a => a.Perfil)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Progresso)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Certificado)
                .FirstOrDefault(a => a.Id == id);
        }

        public void Adicionar(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            _context.SaveChanges();
        }

        public void Atualizar(Aluno aluno)
        {
            _context.Alunos.Update(aluno);
            _context.SaveChanges();
        }
    }
}
