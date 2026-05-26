using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Repositorio
{
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

        public Certificado? BuscarPorId(int id)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(cur => cur.Instrutor)
                .FirstOrDefault(c => c.Id == id);
        }

        public System.Collections.Generic.List<Certificado> BuscarPorAluno(int alunoId)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(cur => cur.Instrutor)
                .Where(c => c.Matricula.AlunoId == alunoId)
                .ToList();
        }

        public Certificado? BuscarPorCodigo(string codigo)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(cur => cur.Instrutor)
                .FirstOrDefault(c => c.CodigoCertificado == codigo);
        }

        public bool ExisteCertificado(int matriculaId)
        {
            return _context.Certificados.Any(c => c.MatriculaId == matriculaId);
        }
    }
}
