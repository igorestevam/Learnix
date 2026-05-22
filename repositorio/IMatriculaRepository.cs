using Learnix.model;

namespace Learnix.Repositorio
{
    public interface IMatriculaRepository
    {
        void Adicionar(Matricula matricula);
        bool ExisteMatriculaAtiva(int alunoId, int cursoId);
        Matricula BuscarPorId(int id);
        int ContarTotal();
    }
}