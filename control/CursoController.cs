using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class CursoController
{
    public List<Curso> ListarTodos()
    {
        using var ctx = new LearnixDbContext();
        return ctx.Cursos
            .Include(c => c.Categoria)
            .Include(c => c.Instrutor)
            .ToList();
    }

    public List<Curso> BuscarPorNome(string termo)
    {
        using var ctx = new LearnixDbContext();
        // SQL puro — requisito da atividade
        return ctx.Cursos
            .FromSqlRaw("SELECT * FROM Cursos WHERE Titulo LIKE {0}", $"%{termo}%")
            .Include(c => c.Categoria)
            .Include(c => c.Instrutor)
            .ToList();
    }

    public Curso? BuscarPorId(int id)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Cursos
            .Include(c => c.Categoria)
            .Include(c => c.Instrutor)
            .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
            .Include(c => c.Atividades)
            .FirstOrDefault(c => c.Id == id);
    }

    public void Adicionar(Curso curso)
    {
        using var ctx = new LearnixDbContext();
        ctx.Cursos.Add(curso);
        ctx.SaveChanges();
    }

    public List<Curso> ListarComMatriculas()
    {
        using var ctx = new LearnixDbContext();
        return ctx.Cursos
            .Include(c => c.Categoria)
            .Include(c => c.Instrutor)
            .Include(c => c.MatriculasAtivas)
            .ToList();
    }

    public List<Curso> ListarPorInstrutor(int instrutorId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Cursos
            .Where(c => c.InstrutorId == instrutorId)
            .Include(c => c.MatriculasAtivas).ThenInclude(m => m.Avaliacoes)
            .Include(c => c.Categoria)
            .ToList();
    }

    public List<Curso> ListarSemInstrutor()
    {
        using var ctx = new LearnixDbContext();
        return ctx.Cursos
            .Where(c => c.InstrutorId == null)
            .Include(c => c.Categoria)
            .ToList();
    }

    public void VincularInstrutor(int cursoId, int instrutorId)
    {
        using var ctx = new LearnixDbContext();
        var curso = ctx.Cursos.Find(cursoId);
        if (curso == null) return;
        curso.InstrutorId = instrutorId;
        ctx.SaveChanges();
    }

    public void DesvincularInstrutor(int cursoId)
    {
        using var ctx = new LearnixDbContext();
        var curso = ctx.Cursos.Find(cursoId);
        if (curso == null) return;
        curso.InstrutorId = null;
        ctx.SaveChanges();
    }

    public void AtualizarDados(int cursoId, string titulo, string descricao, int cargaHoraria)
    {
        using var ctx = new LearnixDbContext();
        var curso = ctx.Cursos.Find(cursoId);
        if (curso == null) return;
        curso.Titulo = titulo;
        curso.Descricao = descricao;
        curso.CargaHoraria = cargaHoraria;
        ctx.SaveChanges();
    }

    public void AdicionarModulo(int cursoId, string titulo)
    {
        using var ctx = new LearnixDbContext();
        var ordem = (ctx.Modulos
            .Where(m => m.CursoId == cursoId)
            .Max(m => (int?)m.Ordem) ?? 0) + 1;
        ctx.Modulos.Add(new Modulo { Titulo = titulo, Ordem = ordem, CursoId = cursoId });
        ctx.SaveChanges();
    }

    public void RemoverModulo(int moduloId)
    {
        using var ctx = new LearnixDbContext();
        var modulo = ctx.Modulos.Include(m => m.Aulas).FirstOrDefault(m => m.Id == moduloId);
        if (modulo == null) return;
        ctx.Modulos.Remove(modulo);
        ctx.SaveChanges();
    }

    public void AdicionarAula(int moduloId, string titulo, int minutos)
    {
        using var ctx = new LearnixDbContext();
        var ordem = (ctx.Aulas
            .Where(a => a.ModuloId == moduloId)
            .Max(a => (int?)a.Ordem) ?? 0) + 1;
        ctx.Aulas.Add(new Aula
        {
            Titulo = titulo,
            VideoUrl = string.Empty,
            Duracao = TimeSpan.FromMinutes(minutos),
            Ordem = ordem,
            ModuloId = moduloId,
        });
        ctx.SaveChanges();
    }

    public void RemoverAula(int aulaId)
    {
        using var ctx = new LearnixDbContext();
        var aula = ctx.Aulas.Find(aulaId);
        if (aula == null) return;
        ctx.Aulas.Remove(aula);
        ctx.SaveChanges();
    }

    public void AtualizarVideoAula(int aulaId, string videoUrl)
    {
        using var ctx = new LearnixDbContext();
        var aula = ctx.Aulas.Find(aulaId);
        if (aula == null) return;
        aula.VideoUrl = videoUrl;
        ctx.SaveChanges();
    }

    public List<AtividadeCurso> ListarAtividades(int cursoId)
    {
        using var ctx = new LearnixDbContext();
        return ctx.AtividadesCursos
            .Where(a => a.CursoId == cursoId)
            .OrderBy(a => a.Id)
            .ToList();
    }

    public void SalvarAtividades(int cursoId, string[] perguntas)
    {
        using var ctx = new LearnixDbContext();
        var existentes = ctx.AtividadesCursos
            .Where(a => a.CursoId == cursoId)
            .OrderBy(a => a.Id)
            .ToList();

        for (int i = 0; i < perguntas.Length; i++)
        {
            if (i < existentes.Count)
                existentes[i].Pergunta = perguntas[i];
            else
                ctx.AtividadesCursos.Add(new AtividadeCurso { Pergunta = perguntas[i], CursoId = cursoId });
        }
        ctx.SaveChanges();
    }
}