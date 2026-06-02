using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class MatriculaController
{
    public List<Matricula> ListarPorAluno(int alunoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
            .Include(m => m.Curso).ThenInclude(c => c.Categoria)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
            .Include(m => m.Progresso)
            .Include(m => m.Certificado)
            .Include(m => m.Avaliacoes)
            .Where(m => m.AlunoId == alunoId)
            .ToList();
    }

    public Matricula? BuscarCompleta(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Progresso)
            .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
            .Include(m => m.Curso).ThenInclude(c => c.Categoria)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos)
                .ThenInclude(mod => mod.Aulas)
            .FirstOrDefault(m => m.Id == matriculaId);
    }

    public List<Matricula> ListarAtivas(int alunoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Where(m => m.AlunoId == alunoId && m.Status != StatusMatricula.Cancelada)
            .Include(m => m.Curso).ThenInclude(c => c.Categoria)
            .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
            .Include(m => m.Progresso)
            .Include(m => m.Avaliacoes)
            .ToList();
    }

    public int? BuscarCertificadoId(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        var matricula = ctx.Matriculas
            .Include(m => m.Certificado)
            .FirstOrDefault(m => m.Id == matriculaId);
        return matricula?.Certificado?.Id;
    }

    public Matricula? BuscarComAtividades(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Include(m => m.Curso).ThenInclude(c => c.Atividades)
            .FirstOrDefault(m => m.Id == matriculaId);
    }

    public void Cancelar(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        var matricula = ctx.Matriculas
            .Include(m => m.Progresso)
            .Include(m => m.Avaliacoes)
            .Include(m => m.Certificado)
            .FirstOrDefault(m => m.Id == matriculaId);

        if (matricula == null) return;

        if (matricula.Progresso != null) ctx.Remove(matricula.Progresso);
        if (matricula.Avaliacoes?.Any() == true) ctx.RemoveRange(matricula.Avaliacoes);
        if (matricula.Certificado != null) ctx.Remove(matricula.Certificado);

        var respostas = ctx.RespostasAtividades.Where(r => r.MatriculaId == matriculaId).ToList();
        if (respostas.Any()) ctx.RespostasAtividades.RemoveRange(respostas);

        matricula.Status = StatusMatricula.Cancelada;
        ctx.SaveChanges();
    }

    public List<Matricula> ListarHistorico(int alunoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Where(m => m.AlunoId == alunoId)
            .ToList();
    }

    public bool Matricular(int alunoId, int cursoId)
    {
        using var ctx = new LearnixDbContext();
        var existente = ctx.Matriculas
            .Include(m => m.Progresso)
            .FirstOrDefault(m => m.AlunoId == alunoId && m.CursoId == cursoId);

        if (existente != null)
        {
            existente.Status = StatusMatricula.Ativa;
            existente.DataMatricula = DateTime.Now;

            if (existente.Progresso != null)
                existente.Progresso.PercentualConcluido = 0;
            else
                existente.Progresso = new Progresso { PercentualConcluido = 0 };

            var aulas = ctx.AulasConcluidas.Where(a => a.MatriculaId == existente.Id).ToList();
            if (aulas.Any()) ctx.AulasConcluidas.RemoveRange(aulas);

            var respostas = ctx.RespostasAtividades.Where(r => r.MatriculaId == existente.Id).ToList();
            if (respostas.Any()) ctx.RespostasAtividades.RemoveRange(respostas);

            ctx.SaveChanges();
            return true;
        }

        ctx.Matriculas.Add(new Matricula
        {
            AlunoId = alunoId,
            CursoId = cursoId,
            Status = StatusMatricula.Ativa,
            DataMatricula = DateTime.Now,
            Progresso = new Progresso { PercentualConcluido = 0 },
        });
        ctx.SaveChanges();
        return false;
    }

    public List<Matricula> ListarPorCursoEStatus(int cursoId, StatusMatricula status)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Include(m => m.Aluno)
            .Where(m => m.CursoId == cursoId && m.Status == status)
            .ToList();
    }

    public List<Matricula> ListarPorCurso(int cursoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Matriculas
            .Where(m => m.CursoId == cursoId)
            .Include(m => m.Aluno)
            .Include(m => m.Avaliacoes)
            .ToList();
    }

    public void AtualizarStatus(int matriculaId, StatusMatricula status)
    {
        using var ctx = new LearnixDbContext();
        var matricula = ctx.Matriculas.Find(matriculaId);
        if (matricula == null) return;
        matricula.Status = status;
        ctx.SaveChanges();
    }
}