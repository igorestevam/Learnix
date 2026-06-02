using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class CertificadoController
{
    public List<Certificado> ListarPorAluno(int alunoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Certificados
            .Include(c => c.Matricula).ThenInclude(m => m.Aluno)
            .Include(c => c.Matricula).ThenInclude(m => m.Curso).ThenInclude(cur => cur.Instrutor)
            .Where(c => c.Matricula.AlunoId == alunoId)
            .OrderByDescending(c => c.DataEmissao)
            .ToList();
    }

    public Certificado? BuscarPorCodigo(string codigo)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Certificados
            .Include(c => c.Matricula).ThenInclude(m => m.Aluno)
            .Include(c => c.Matricula).ThenInclude(m => m.Curso)
            .FirstOrDefault(c => c.CodigoCertificado == codigo);
    }
}