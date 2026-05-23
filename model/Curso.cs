using System.Collections.Generic;

namespace Learnix.model
{
    /// <summary>
    /// Classe base abstrata para cursos. Subclasses: CursoExatas, CursoHumanas.
    /// Telas: TelaMenu (Titulo, Descricao, CargaHoraria, Preco, Instrutor, Categoria),
    ///        TelaMeusCursos (Titulo, CargaHoraria, Categoria, Instrutor, DataMatricula),
    ///        TelaAulas (Titulo, Instrutor, Categoria, CargaHoraria, Descricao),
    ///        TelaCertificados (Titulo, CargaHoraria, Instrutor)
    /// </summary>
    public abstract class Curso
    {
        public int Id { get; set; }

        // Exibido em todas as telas que listam cursos
        public string Titulo { get; set; } = null!;

        // Exibido na TelaMenu (descrição do curso) e TelaAulas
        public string Descricao { get; set; } = null!;

        // Exibido em TelaMeusCursos, TelaMenu e TelaCertificados
        public int CargaHoraria { get; set; }

        // Exibido na TelaMenu como valor do curso
        public decimal Preco { get; set; }

        // Categoria (Exatas/Humanas/Tecnologia) — exibida como tag colorida nas telas
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Nome do instrutor exibido em TelaMenu, TelaMeusCursos, TelaAulas, TelaHome
        public int InstrutorId { get; set; }
        public Instrutor Instrutor { get; set; } = null!;

        // Módulos que compõem o curso — usados na TelaAulas e TelaPlayer
        public List<Modulo> Modulos { get; set; } = null!;

        // Matrículas ativas neste curso — usadas no ProgressoService
        public List<Matricula> MatriculasAtivas { get; set; } = null!;

        protected Curso()
        {
            Modulos = new List<Modulo>();
            MatriculasAtivas = new List<Matricula>();
        }

        protected Curso(int id, string titulo, int cargaHoraria)
        {
            Id = id;
            Titulo = titulo;
            CargaHoraria = cargaHoraria;
            Modulos = new List<Modulo>();
            MatriculasAtivas = new List<Matricula>();
        }
    }
}
