using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class AlunoController
{
    public Aluno? BuscarComHistorico(int id)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Alunos
            .Include(x => x.HistoricoMatriculas).ThenInclude(m => m.Curso)
            .FirstOrDefault(x => x.Id == id);
    }

    public void AtualizarPerfil(int alunoId, string nome, string email)
    {
        using var ctx = new LearnixDbContext();
        var aluno = ctx.Alunos.Find(alunoId);
        if (aluno == null) return;
        aluno.Nome = nome;
        aluno.Email = email;
        ctx.SaveChanges();
    }

    public PerfilDeAprendizagem? ObterPerfil(int alunoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Alunos
            .Where(a => a.Id == alunoId)
            .Select(a => a.Perfil)
            .FirstOrDefault();
    }

    public void AtualizarPerfilAprendizagem(int alunoId, string estilo, string ritmo)
    {
        using var ctx = new LearnixDbContext();
        var aluno = ctx.Alunos
            .Include(a => a.Perfil)
            .FirstOrDefault(a => a.Id == alunoId);
        if (aluno?.Perfil == null) return;
        aluno.Perfil.EstiloPredominante = estilo;
        aluno.Perfil.RitmoSugerido = ritmo;
        ctx.SaveChanges();
    }
}
