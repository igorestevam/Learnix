using Learnix.data;
using Learnix.model;

namespace Learnix.control;

public class CategoriaController
{
    public List<Categoria> ListarTodas()
    {
        using var ctx = new LearnixDbContext();
        return ctx.Categorias.ToList();
    }

    public bool Adicionar(string nome, string descricao)
    {
        using var ctx = new LearnixDbContext();
        if (ctx.Categorias.Any(c => c.Nome.ToLower() == nome.ToLower())) return false;
        ctx.Categorias.Add(new Categoria { Nome = nome, Descricao = descricao });
        ctx.SaveChanges();
        return true;
    }
}
