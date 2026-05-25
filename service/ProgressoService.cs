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
            // Carrega a matricula trazendo o progresso e a estrutura de aulas do curso
            Matricula? matricula = _context.Matriculas
                .Include(m => m.Progresso)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                        .ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matriculaId);

            if (matricula == null || matricula.Progresso == null || matricula.Curso == null)
                return false;

            // Idempotencia: verifica se esta aula ja foi concluida nesta matricula.
            // A PK composta (MatriculaId + AulaId) impede duplicatas no banco,
            // mas verificamos antes para evitar excecao de constraint desnecessaria.
            bool jaRegistrada = _context.AulasConcluidas
                .Any(ac => ac.MatriculaId == matriculaId && ac.AulaId == aulaId);

            if (jaRegistrada)
                return false;

            // Verifica se a aula pertence de fato a este curso
            bool aulaExisteNoCurso = matricula.Curso.Modulos
                .SelectMany(m => m.Aulas)
                .Any(a => a.Id == aulaId);

            if (!aulaExisteNoCurso)
                return false;

            // Registra a conclusao desta aula especifica
            _context.AulasConcluidas.Add(new AulaConcluida(matriculaId, aulaId));

            // Recalcula o progresso com base nas aulas efetivamente concluidas
            int totalAulas = matricula.Curso.Modulos.Sum(m => m.Aulas.Count);

            if (totalAulas == 0)
                return false;

            // Conta diretamente do banco para garantir consistencia
            int aulasConcluidas = _context.AulasConcluidas
                .Count(ac => ac.MatriculaId == matriculaId);

            // +1 porque o Add acima ainda nao foi salvo (SaveChanges ainda nao foi chamado)
            int totalConcluidas = aulasConcluidas + 1;

            double percentual = ((double)totalConcluidas / totalAulas) * 100.0;
            matricula.Progresso.AulasConcluidas = totalConcluidas;
            matricula.Progresso.PercentualConcluido = percentual;
            matricula.Progresso.UltimaAtualizacao = DateTime.Now;

            // Regra de Negocio: Se concluiu 100%, finaliza a matricula e emite o certificado
            if (percentual >= 100.0 && matricula.Certificado == null)
            {
                matricula.Status = StatusMatricula.Concluida;

                string codigoUnico = "LX-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                Certificado novoCertificado = new Certificado
                {
                    CodigoCertificado = codigoUnico,
                    DataEmissao = DateTime.Now,
                    MatriculaId = matricula.Id
                };
                _context.Certificados.Add(novoCertificado);
            }

            _context.SaveChanges();
            return true;
        }
    }
}