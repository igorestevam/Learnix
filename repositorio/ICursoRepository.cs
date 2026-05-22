using Learnix.model;
using System.Collections.Generic;

namespace Learnix.Repositorio
{
    public interface ICursoRepository
    {
        List<Curso> BuscarCursosPorNome(string termoPesquisa);
    }
}