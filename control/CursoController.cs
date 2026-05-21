using Learnix.model;
using Learnix.repositorio;
using System.Collections.Generic;

namespace Learnix.control
{
    public class CursoController
    {
        private readonly CursoRepository _cursoRepository;

        public CursoController()
        {
            _cursoRepository = new CursoRepository();
        }

        public void SalvarCurso(Curso curso)
        {
            _cursoRepository.SalvarNoBanco(curso);
        }

        public List<Curso> ListarCursos()
        {
            return _cursoRepository.RecuperarTodosOsDados();
        }

        public Curso? BuscarCursoPorId(int id)
        {
            return _cursoRepository.RecuperarDadoEspecifico(id);
        }

        public void ExcluirCurso(int id)
        {
            _cursoRepository.DeletarDado(id);
        }

        public List<Curso> BuscarViaSql(string termo)
        {
            return _cursoRepository.BuscarCursosPorNomeSqlPuro(termo);
        }
    }
}