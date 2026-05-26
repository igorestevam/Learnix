using System.Collections.Generic;
using Learnix.model;

namespace Learnix.Services
{
    /// <summary>
    /// Contrato do servico de Certificado. Usado pela TelaCertificados.
    /// </summary>
    public interface ICertificadoService
    {
        /// <summary>Lista todos os certificados de um Aluno com dados completos para exibicao.</summary>
        List<Certificado> ListarPorAluno(int alunoId);

        /// <summary>Busca um certificado por codigo unico (validacao).</summary>
        Certificado? BuscarPorCodigo(string codigo);

        /// <summary>Conta quantos certificados o aluno possui.</summary>
        int ContarPorAluno(int alunoId);
    }
}
