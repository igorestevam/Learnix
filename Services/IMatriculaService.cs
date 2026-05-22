using Learnix.model;

namespace Learnix.Services
{
    public interface IMatriculaService
    {
        Matricula CriarMatricula(int alunoId, int cursoId);
    }
}