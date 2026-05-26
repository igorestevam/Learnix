using System.Collections.Generic;
using Learnix.model;

namespace Learnix.Repositorio
{
    /// <summary>
    /// Contrato de persistencia para Certificado.
    /// Usado pela TelaCertificados (via CertificadoService).
    /// </summary>
    public interface ICertificadoRepository
    {
        /// <summary>Busca um certificado pelo Id (com Matricula, Aluno, Curso, Instrutor carregados).</summary>
        Certificado? BuscarPorId(int id);

        /// <summary>Lista todos os certificados de um Aluno especifico.</summary>
        List<Certificado> BuscarPorAluno(int alunoId);

        /// <summary>Busca um certificado pelo codigo unico (ex: 'LX-A3F9B2').</summary>
        Certificado? BuscarPorCodigo(string codigo);

        /// <summary>Adiciona um novo certificado ao banco.</summary>
        void Adicionar(Certificado certificado);
    }
}
