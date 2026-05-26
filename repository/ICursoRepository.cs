using Learnix.model;
using System.Collections.Generic;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Contrato de persistência para Curso.
    /// Usado por: TelaMenu (listar todos os cursos, buscar por nome/categoria),
    ///            MatriculaService (buscar curso por Id para criar matrícula).
    /// </summary>
    public interface ICursoRepository
    {
        /// <summary>Retorna todos os cursos cadastrados com Instrutor e Categoria carregados.</summary>
        List<Curso> BuscarTodos();

        /// <summary>Busca um curso específico pelo Id com todos os dados carregados.</summary>
        Curso? BuscarPorId(int id);

        /// <summary>Busca cursos cujo título contenha o termo de pesquisa.</summary>
        List<Curso> BuscarCursosPorNome(string termoPesquisa);

        /// <summary>Busca cursos por categoria (Exatas, Humanas, Tecnologia).</summary>
        List<Curso> BuscarPorCategoria(string nomeCategoria);
    }
}
