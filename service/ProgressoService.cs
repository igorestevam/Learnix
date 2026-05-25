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
            // Carrega a matrícula trazendo o progresso e a estrutura de aulas do curso
            Matricula matricula = _context.Matriculas
                .Include(m => m.Progresso)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                        .ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matriculaId);

            if (matricula == null || matricula.Progresso == null)
            {
                return false;
            }

            // Calcula o total de aulas existentes no curso
            int totalAulas = 0;
            foreach (Modulo modulo in matricula.Curso.Modulos)
            {
                totalAulas += modulo.Aulas.Count;
            }

            if (totalAulas == 0)
            {
                return false;
            }

            // Incrementa a quantidade de aulas feitas respeitando o limite máximo
            if (matricula.Progresso.AulasConcluidas < totalAulas)
            {
                matricula.Progresso.AulasConcluidas += 1;
            }

            // Atualiza os indicadores de progresso
            double percentual = ((double)matricula.Progresso.AulasConcluidas / totalAulas) * 100.0;
            matricula.Progresso.PercentualConcluido = percentual;
            matricula.Progresso.UltimaAtualizacao = DateTime.Now;

            // Regra de Negócio: Se concluiu 100%, finaliza a matrícula e emite o certificado
            if (percentual >= 100.0)
            {
                matricula.Status = StatusMatricula.Concluida;

                int proximoCertificadoId = _context.Certificados.Count() + 1;
                string codigoUnico = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                Certificado novoCertificado = new Certificado(proximoCertificadoId, codigoUnico);
                matricula.Certificado = novoCertificado;
            }

            _context.SaveChanges();
            return true;
        }
    }
}