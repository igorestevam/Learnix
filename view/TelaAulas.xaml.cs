using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.model;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = "Aluno";
        private Matricula _matricula;

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula, string nomeAluno)
        {
            _matricula = matricula;
            _nomeAluno = nomeAluno;
            TxtNomeCurso.Text = matricula.Curso?.Titulo;
            TxtProfessor.Text = matricula.Curso?.Instrutor?.Nome;
            TxtCategoria.Text = matricula.Curso?.Categoria?.Nome;
            TxtCargaHoraria.Text = matricula.Curso?.CargaHoraria + "h";
            TxtDescricao.Text = matricula.Curso?.Descricao;
            TxtProgresso.Text = matricula.Progresso?.PercentualConcluido + "%";
            Sidebar.DefinirAluno(nomeAluno);
            CarregarModulos(matricula.Curso?.Modulos);
        }

        private void CarregarModulos(System.Collections.Generic.List<Modulo> modulos)
        {
            PainelModulos.Children.Clear();
            if (modulos == null) return;

            foreach (var modulo in modulos)
            {
                // Cabeçalho do módulo
                var header = new TextBlock
                {
                    Text = $"Módulo {modulo.Ordem}: {modulo.Titulo}",
                    FontSize = 15,
                    FontWeight = System.Windows.FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(0, 12, 0, 4)
                };
                PainelModulos.Children.Add(header);

                foreach (var aula in modulo.Aulas)
                {
                    var card = CriarCardAula(aula);
                    PainelModulos.Children.Add(card);
                }
            }
        }

        private UIElement CriarCardAula(Aula aula)
        {
            var border = new Border
            {
                Margin = new Thickness(0, 4, 0, 4),
                Tag = aula,
                Opacity = 1.0
            };

            var grid = new Grid();
            var sp = new StackPanel { Margin = new Thickness(12) };

            var titulo = new TextBlock
            {
                Text = $"{aula.Ordem}. {aula.Titulo}",
                FontSize = 14,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };
            var duracao = new TextBlock
            {
                Text = $"Duração: {aula.Duracao}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.LightGray
            };
            var btnAssistir = new Button { Content = "Assistir", Tag = aula, Margin = new Thickness(0, 6, 0, 0) };
            btnAssistir.Click += BtnAssistir_Click;

            sp.Children.Add(titulo);
            sp.Children.Add(duracao);
            sp.Children.Add(btnAssistir);
            grid.Children.Add(sp);
            border.Child = grid;

            // Clique no card também abre o player
            border.MouseLeftButtonUp += (s, e) =>
            {
                if (s is Border b && b.Tag is Aula a)
                    AbrirPlayer(a);
            };

            return border;
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos(_nomeAluno);
        }

        private void BtnAssistir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Aula aula)
                AbrirPlayer(aula);
        }

        private void AbrirPlayer(Aula aula)
        {
            var player = new TelaPlayer();
            player.DefinirAula(aula, _matricula, _nomeAluno);
            (Application.Current.MainWindow as MainWindow)?.MostrarTela(player, _nomeAluno);
        }
    }
}
