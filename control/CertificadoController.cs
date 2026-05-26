using System.Collections.Generic;
using Learnix.model;
using Learnix.Services;

namespace Learnix.Controllers
{
    /// <summary>
    /// Controller responsavel por orquestrar a exibicao e validacao de certificados.
    /// Usado pela TelaCertificados.
    /// </summary>
    public class CertificadoController
    {
        private readonly ICertificadoService _certificadoService;

        public CertificadoController(ICertificadoService certificadoService)
        {
            _certificadoService = certificadoService;
        }

        public List<Certificado> ListarDoAluno(int alunoId)
            => _certificadoService.ListarPorAluno(alunoId);

        public Certificado? Validar(string codigo)
            => _certificadoService.BuscarPorCodigo(codigo);

        public int ContarDoAluno(int alunoId)
            => _certificadoService.ContarPorAluno(alunoId);
    }
}
