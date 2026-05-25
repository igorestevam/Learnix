using System.Collections.Generic;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Contrato do servico de Curso. Usado pela TelaMenu para listar e filtrar.
    /// </summary>
    public interface ICursoService
    {
        /// <summary>Lista todos os cursos com Categoria e Instrutor carregados.</summary>
        List<Curso> ListarTodos();

        /// <summary>Lista cursos por nome de categoria (Exatas/Humanas/Tecnologia).</summary>
        List<Curso> ListarPorCategoria(string nomeCategoria);

        /// <summary>Busca cursos por termo no titulo.</summary>
        List<Curso> BuscarPorTermo(string termo);

        /// <summary>Busca um curso por Id com todos os relacionamentos.</summary>
        Curso? BuscarPorId(int id);
    }
}
