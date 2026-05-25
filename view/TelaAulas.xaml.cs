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

        public void DefinirMatricula(Matricula matricula)
        {
            _matricula = matricula;
            if (matricula?.Aluno != null)
            {
                _nomeAluno = matricula.Aluno.Nome;
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);
            }

            if (matricula?.Curso != null)
            {
                TxtNomeCurso.Text = matricula.Curso.Titulo;
                if (matricula.Curso.Instrutor != null)
                    TxtProfessor.Text = "Prof. " + matricula.Curso.Instrutor.Nome;
                if (matricula.Curso.Categoria != null)
                    TxtCategoria.Text = matricula.Curso.Categoria.Nome;
                TxtCargaHoraria.Text = matricula.Curso.CargaHoraria + "h";
                TxtDescricao.Text = matricula.Curso.Descricao ?? string.Empty;
            }
            TxtProgresso.Text = ((int)Math.Round(matricula?.PercentualConcluido ?? 0)) + "%";
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
                main.MostrarTela(player);
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
                main.MostrarTela(player);
            }
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }
    }
}
