using System.Collections.Generic;
using Learnix.model;

namespace Learnix.Repositorio
{
    public interface IMatriculaRepository
    {
        void Adicionar(Matricula matricula);
        bool ExisteMatriculaAtiva(int alunoId, int cursoId);
        Matricula? BuscarPorId(int id);
        int ContarTotal();

        /// <summary>
        /// Busca todas as matriculas de um aluno especifico, com Curso, Categoria, Instrutor,
        /// Progresso, Certificado, Modulos, Aulas e Avaliacoes carregados. Usado por TelaMeusCursos.
        /// </summary>
        List<Matricula> BuscarPorAluno(int alunoId);
    }
}
