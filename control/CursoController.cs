using Learnix.model;
using Learnix.Repositorio;
using System.Collections.Generic;

namespace Learnix.Controllers
{
    public class CursoController
    {
        private readonly ICursoRepository _cursoRepository;

        // Injeção de Dependência do Repositório
        public CursoController(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public List<Curso> BuscarCursos(string termoPesquisa)
        {
            List<Curso> cursos = _cursoRepository.BuscarCursosPorNome(termoPesquisa);
            return cursos;
        }
    }
}