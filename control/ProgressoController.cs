using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class ProgressoController
{
    public bool ConcluirAula(int matriculaId, int aulaId)
    {
        using var ctx = new LearnixDbContext();
        var matricula = ctx.Matriculas
            .Include(m => m.Progresso)
            .Include(m => m.Certificado)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
            .FirstOrDefault(m => m.Id == matriculaId);

        if (matricula?.Progresso == null || matricula.Curso == null) return false;
        if (ctx.AulasConcluidas.Any(ac => ac.MatriculaId == matriculaId && ac.AulaId == aulaId)) return false;

        bool aulaExiste = matricula.Curso.Modulos.SelectMany(m => m.Aulas).Any(a => a.Id == aulaId);
        if (!aulaExiste) return false;

        ctx.AulasConcluidas.Add(new AulaConcluida(matriculaId, aulaId));

        int total = matricula.Curso.Modulos.Sum(m => m.Aulas.Count);
        int concluidas = ctx.AulasConcluidas.Count(ac => ac.MatriculaId == matriculaId) + 1;
        double percentual = total > 0 ? (double)concluidas / total * 100.0 : 0;

        matricula.Progresso.PercentualConcluido = percentual;
        matricula.Progresso.UltimaAtualizacao = DateTime.Now;

        ctx.SaveChanges();
        return true;
    }

    public HashSet<int> ObterAulasConcluidas(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.AulasConcluidas
            .Where(ac => ac.MatriculaId == matriculaId)
            .Select(ac => ac.AulaId)
            .ToHashSet();
    }

    public double ObterPercentual(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Progressos
            .Where(p => p.MatriculaId == matriculaId)
            .Select(p => p.PercentualConcluido)
            .FirstOrDefault();
    }
}