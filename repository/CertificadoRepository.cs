using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Implementação de persistência para Certificado usando Entity Framework + LocalDB.
    /// </summary>
    public class CertificadoRepository : ICertificadoRepository
    {
        private readonly LearnixDbContext _context;

        public CertificadoRepository(LearnixDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Certificado certificado)
        {
            _context.Certificados.Add(certificado);
            _context.SaveChanges();
        }

        public List<Certificado> BuscarPorAluno(int alunoId)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(curso => curso.Instrutor)
                .Where(c => c.Matricula.AlunoId == alunoId)
                .ToList();
        }

        public Certificado? BuscarPorCodigo(string codigoCertificado)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                .FirstOrDefault(c => c.CodigoCertificado == codigoCertificado);
        }

        public bool ExisteCertificado(int matriculaId)
        {
            return _context.Certificados
                .Any(c => c.MatriculaId == matriculaId);
        }
    }
}
