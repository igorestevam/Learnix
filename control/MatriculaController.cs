using System;
using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    public class MatriculaController
    {
        private readonly IMatriculaService _matriculaService;

        // Controladores delegam a regra de negócio para as camadas de serviço
        public MatriculaController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        public void MatricularAluno(int alunoId, int cursoId)
        {
            Matricula novaMatricula = _matriculaService.CriarMatricula(alunoId, cursoId);

            if (novaMatricula == null)
            {
                Console.WriteLine("Erro: Não foi possível efetuar a matrícula.");
                return;
            }

            Console.WriteLine($"Matrícula número {novaMatricula.Id} ativada com sucesso!");
        }
    }
}