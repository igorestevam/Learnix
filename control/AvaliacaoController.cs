using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class AvaliacaoController
{
    public List<RespostaAtividade> ListarRespostasPorMatriculas(List<int> matriculaIds)
    {
        using var ctx = new LearnixDbContext();
        return ctx.RespostasAtividades
            .Where(r => matriculaIds.Contains(r.MatriculaId))
            .ToList();
    }

    public void SalvarNotasAvaliacao(int matriculaId, List<(string titulo, double nota)> notas)
    {
        using var ctx = new LearnixDbContext();
        var existentes = ctx.Avaliacoes
            .Where(a => a.MatriculaId == matriculaId)
            .ToList();

        foreach (var (titulo, nota) in notas)
        {
            var existente = existentes.FirstOrDefault(a => a.Titulo == titulo);
            if (existente != null)
            {
                existente.Nota = nota;
                existente.DataRealizacao = DateTime.Now;
            }
            else
            {
                ctx.Avaliacoes.Add(new Avaliacao
                {
                    MatriculaId = matriculaId,
                    Titulo = titulo,
                    Nota = nota,
                    DataRealizacao = DateTime.Now,
                });
            }
        }
        ctx.SaveChanges();
    }

    public List<AtividadeCurso> ListarAtividadesPendentes(List<int> matriculaIds, List<int> cursoIds)
    {
        using var ctx = new LearnixDbContext();
        var respondidas = ctx.RespostasAtividades
            .Where(r => matriculaIds.Contains(r.MatriculaId))
            .Select(r => r.AtividadeCursoId)
            .ToHashSet();

        return ctx.AtividadesCursos
            .Include(a => a.Curso)
            .Where(a => cursoIds.Contains(a.CursoId) && !respondidas.Contains(a.Id))
            .ToList();
    }

    public List<RespostaAtividade> ListarRespostas(int matriculaId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.RespostasAtividades
            .Include(r => r.AtividadeCurso)
            .Where(r => r.MatriculaId == matriculaId)
            .ToList();
    }

    public (bool aprovado, decimal media) SalvarNotas(int matriculaId, Dictionary<int, decimal> notas)
    {
        using var ctx = new LearnixDbContext();
        var matricula = ctx.Matriculas.Include(m => m.Curso).FirstOrDefault(m => m.Id == matriculaId);
        if (matricula == null) return (false, 0);

        decimal soma = 0;
        foreach (var (respostaId, nota) in notas)
        {
            var resposta = ctx.RespostasAtividades.Find(respostaId);
            if (resposta != null) { resposta.Nota = nota; soma += nota; }
        }

        decimal media = notas.Count > 0 ? soma / notas.Count : 0;
        bool aprovado = media >= 7.0m;

        if (aprovado)
        {
            matricula.Status = StatusMatricula.Concluida;
            if (!ctx.Certificados.Any(c => c.MatriculaId == matricula.Id))
            {
                ctx.Certificados.Add(new Certificado
                {
                    MatriculaId = matricula.Id,
                    CodigoCertificado = "LX-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                    DataEmissao = DateTime.Now,
                });
            }
        }
        else
        {
            matricula.Status = StatusMatricula.Reprovada;
        }

        ctx.SaveChanges();
        return (aprovado, media);
    }

    public bool EnviarRespostas(int matriculaId,
        int atividade1Id, string resp1,
        int atividade2Id, string resp2,
        int atividade3Id, string resp3)
    {
        using var ctx = new LearnixDbContext();
        if (ctx.RespostasAtividades.Any(r => r.MatriculaId == matriculaId)) return false;

        ctx.RespostasAtividades.AddRange(
            new RespostaAtividade { MatriculaId = matriculaId, AtividadeCursoId = atividade1Id, Resposta = resp1 },
            new RespostaAtividade { MatriculaId = matriculaId, AtividadeCursoId = atividade2Id, Resposta = resp2 },
            new RespostaAtividade { MatriculaId = matriculaId, AtividadeCursoId = atividade3Id, Resposta = resp3 }
        );

        var matricula = ctx.Matriculas.Find(matriculaId);
        if (matricula != null) matricula.Status = StatusMatricula.AguardandoCorrecao;

        ctx.SaveChanges();
        return true;
    }
}