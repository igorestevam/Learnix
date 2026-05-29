using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = string.Empty;
        private Matricula? _matricula;

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula, string nomeAluno)
        {
            _nomeAluno = string.IsNullOrWhiteSpace(nomeAluno)
                ? (matricula?.Aluno?.Nome ?? "Aluno")
                : nomeAluno;

            using var db = new LearnixDbContext();
            _matricula = db.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Progresso)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso).ThenInclude(c => c.Categoria)
                .Include(m => m.Curso).ThenInclude(c => c.Modulos)
                    .ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matricula.Id)
                ?? matricula;

            Sidebar?.DefinirAluno(_nomeAluno);
            PopularDadosCurso();
        }

        public void DefinirMatricula(Matricula matricula)
        {
            DefinirMatricula(matricula, matricula?.Aluno?.Nome ?? "Aluno");
        }

        private void PopularDadosCurso()
        {
            if (_matricula?.Curso == null) return;

            TxtNomeCurso.Text = _matricula.Curso.Titulo;
            TxtProfessor.Text = _matricula.Curso.Instrutor != null
                                   ? "Prof. " + _matricula.Curso.Instrutor.Nome : "";
            TxtCategoria.Text = _matricula.Curso.Categoria?.Nome ?? "";
            TxtCargaHoraria.Text = _matricula.Curso.CargaHoraria + "h";
            TxtDescricao.Text = _matricula.Curso.Descricao ?? "";

            double pct = _matricula?.Progresso?.PercentualConcluido ?? 0.0;
            TxtProgresso.Text = ((int)Math.Round(pct)) + "%";
        }

        private void AulaCard_Click(object sender, MouseButtonEventArgs e)
        {
            AbrirPlayer();
        }

        private void BtnAssistir_Click(object sender, RoutedEventArgs e)
        {
            AbrirPlayer();
        }

        private void AbrirPlayer()
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarAulaNoPlayer();
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }
    }
}