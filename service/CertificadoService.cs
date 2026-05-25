using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Servico de Certificado. Encapsula consultas necessarias para a TelaCertificados.
    /// </summary>
    public class CertificadoService : ICertificadoService
    {
        private readonly LearnixDbContext _context;

        public CertificadoService(LearnixDbContext context)
        {
            _context = context;
        }

        public List<Certificado> ListarPorAluno(int alunoId)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(curso => curso.Instrutor)
                .Where(c => c.Matricula.AlunoId == alunoId)
                .OrderByDescending(c => c.DataEmissao)
                .ToList();
        }

        public Certificado? BuscarPorCodigo(string codigo)
        {
            return _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(curso => curso.Instrutor)
                .FirstOrDefault(c => c.CodigoCertificado == codigo);
        }

        public int ContarPorAluno(int alunoId)
        {
            return _context.Certificados
                .Count(c => c.Matricula.AlunoId == alunoId);
        }
    }
}
