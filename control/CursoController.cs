using Learnix.model;
using Learnix.Repositorio;
using System.Collections.Generic;

namespace Learnix.Controllers
{
    public class CursoController
    {
        private readonly CursoRepository _cursoRepository;

        public CursoController()
        {
            // O controller instancia o repositório
            _cursoRepository = new CursoRepository();
        }

        public List<Curso> BuscarCursos(string termoPesquisa)
        {
            // O controller apenas se comunica com o repositório, delegando a busca
            return _cursoRepository.BuscarCursosPorNome(termoPesquisa);
        }
    }
}