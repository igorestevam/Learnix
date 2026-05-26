using System;
using Learnix.Services;

namespace Learnix.Controllers
{
    public class AulaController
    {
        private readonly IProgressoService _progressoService;

        public AulaController(IProgressoService progressoService)
        {
            _progressoService = progressoService;
        }

        public void ConcluirAula(int matriculaId, int aulaId)
        {
            // Registra que o aluno assistiu à aula e atualiza a entidade Progresso
            bool sucesso = _progressoService.RegistrarConclusaoAula(matriculaId, aulaId);

            if (sucesso)
            {
                Console.WriteLine("Aula marcada como concluída! Progresso atualizado com sucesso.");
            }
            else
            {
                Console.WriteLine("Erro: Falha ao registrar a conclusão da aula.");
            }
        }
    }
}