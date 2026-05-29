using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class ProgressoService : IProgressoService
    {
        private readonly LearnixDbContext _context;

        public ProgressoService(LearnixDbContext context)
        {
            _context = context;
        }

        public bool RegistrarConclusaoAula(int matriculaId, int aulaId)
        {
            // Carrega a matrícula com progresso e estrutura de aulas
            var matricula = _context.Matriculas
                .Include(m => m.Progresso)
                .Include(m => m.Certificado)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                        .ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matriculaId);

            if (matricula == null || matricula.Progresso == null || matricula.Curso == null)
                return false;

            // Idempotência: verifica se esta aula já foi concluída
            // Usa a tabela AulaConcluida (PK composta MatriculaId + AulaId)
            bool jaRegistrada = _context.Set<AulaConcluida>()
                .Any(ac => ac.MatriculaId == matriculaId && ac.AulaId == aulaId);

            if (jaRegistrada)
                return false;

            // Verifica se a aula pertence a este curso
            bool aulaExisteNoCurso = matricula.Curso.Modulos
                .SelectMany(m => m.Aulas)
                .Any(a => a.Id == aulaId);

            if (!aulaExisteNoCurso)
                return false;

            // Registra a conclusão desta aula
            _context.Set<AulaConcluida>().Add(new AulaConcluida(matriculaId, aulaId));

            // Recalcula o progresso
            int totalAulas = matricula.Curso.Modulos.Sum(m => m.Aulas.Count);
            if (totalAulas == 0) return false;

            // Conta do banco (+1 porque o Add ainda não foi salvo)
            int jaConcluidasNoBanco = _context.Set<AulaConcluida>()
                .Count(ac => ac.MatriculaId == matriculaId);

            int totalConcluidas = jaConcluidasNoBanco + 1;
            double percentual   = ((double)totalConcluidas / totalAulas) * 100.0;

            matricula.Progresso.AulasConcluidas      = totalConcluidas;
            matricula.Progresso.PercentualConcluido  = percentual;
            matricula.Progresso.UltimaAtualizacao    = DateTime.Now;

            // Regra de negócio: 100% → conclui matrícula e emite certificado
            if (percentual >= 100.0 && matricula.Certificado == null)
            {
                matricula.Status = StatusMatricula.Concluida;

                _context.Certificados.Add(new Certificado
                {
                    CodigoCertificado = "LX-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                    DataEmissao       = DateTime.Now,
                    MatriculaId       = matricula.Id
                });
            }

            _context.SaveChanges();
            return true;
        }
    }
}
