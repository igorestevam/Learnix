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

        /// <summary>
        /// Sobrecarga compatibilizada com MainWindow.MostrarAulas (que passa nome do aluno
        /// extraido de _usuarioLogado em vez de depender de matricula.Aluno).
        /// </summary>
        public void DefinirMatricula(Matricula matricula, string nomeAluno)
        {
            _matricula = matricula;
            _nomeAluno = string.IsNullOrWhiteSpace(nomeAluno)
                ? (matricula?.Aluno?.Nome ?? "Aluno")
                : nomeAluno;

            Sidebar?.DefinirAluno(_nomeAluno);

            PopularDadosCurso();
        }

        /// <summary>
        /// Mantida por compatibilidade. Usa o nome contido em matricula.Aluno (se carregado).
        /// </summary>
        public void DefinirMatricula(Matricula matricula)
        {
            _matricula = matricula;
            _nomeAluno = matricula?.Aluno?.Nome ?? "Aluno";

            Sidebar?.DefinirAluno(_nomeAluno);

            PopularDadosCurso();
        }

        private void PopularDadosCurso()
        {
            if (_matricula?.Curso != null)
            {
                TxtNomeCurso.Text = _matricula.Curso.Titulo;
                if (_matricula.Curso.Instrutor != null)
                    TxtProfessor.Text = "Prof. " + _matricula.Curso.Instrutor.Nome;
                if (_matricula.Curso.Categoria != null)
                    TxtCategoria.Text = _matricula.Curso.Categoria.Nome;
                TxtCargaHoraria.Text = _matricula.Curso.CargaHoraria + "h";
                TxtDescricao.Text = _matricula.Curso.Descricao ?? string.Empty;
            }

            // Le o percentual do Progresso vinculado a matricula (nao do model Matricula direto)
            double percentual = _matricula?.Progresso?.PercentualConcluido ?? 0.0;
            TxtProgresso.Text = ((int)Math.Round(percentual)) + "%";
        }

        private void AulaCard_Click(object sender, MouseButtonEventArgs e)
        {
            // O XAML tem 3 cards estaticos com Tag="1"/"2"/"3" representando a ordem da aula
            if (sender is not FrameworkElement fe || fe.Tag == null) return;
            if (_matricula?.Curso == null) return;

            if (!int.TryParse(fe.Tag.ToString(), out int ordem)) return;
            var aulas = _matricula.Curso.Modulos.SelectMany(m => m.Aulas).OrderBy(a => a.Id).ToList();
            if (ordem < 1 || ordem > aulas.Count) return;

            var aula = aulas[ordem - 1];
            var main = Application.Current.MainWindow as MainWindow;
            if (main != null)
            {
                var player = new TelaPlayer();
                player.DefinirAula(_matricula, aula);
                main.MostrarTela(player, _nomeAluno);
            }
        }

        private void BtnAssistir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null) return;
            if (_matricula?.Curso == null) return;

            if (!int.TryParse(btn.Tag.ToString(), out int ordem)) return;
            var aulas = _matricula.Curso.Modulos.SelectMany(m => m.Aulas).OrderBy(a => a.Id).ToList();
            if (ordem < 1 || ordem > aulas.Count)
            {
                MessageBox.Show("Aula nao disponivel.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var aula = aulas[ordem - 1];
            var main = Application.Current.MainWindow as MainWindow;
            if (main != null)
            {
                var player = new TelaPlayer();
                player.DefinirAula(_matricula, aula);
                main.MostrarTela(player, _nomeAluno);
            }
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }
    }
}