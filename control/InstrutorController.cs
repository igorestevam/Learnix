using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.control;

public class InstrutorController
{
    public Instrutor? BuscarComCursos(int id)
    {
        using var ctx = new LearnixDbContext();
        return ctx.Instrutores
            .Include(i => i.Cursos)
            .FirstOrDefault(i => i.Id == id);
    }
}
