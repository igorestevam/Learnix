using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private string _categoriaAtiva = "Todos";
        private const string PlaceholderBusca = "Buscar curso...";
        private Aluno? _alunoLogado;

        public TelaMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Recebe o aluno logado para que possa ser matriculado em cursos reais.
        /// </summary>
        public void DefinirAluno(Aluno aluno)
        {
            _alunoLogado = aluno;
        }

        // -- Busca --

        private void TxtBusca_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtBusca.Text == PlaceholderBusca)
            {
                TxtBusca.Text = "";
                TxtBusca.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void TxtBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBusca.Text))
            {
                TxtBusca.Text = PlaceholderBusca;
                TxtBusca.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#9E8FC0"));
            }
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtBusca.Text == PlaceholderBusca) return;
            AplicarFiltros();
        }

        // -- Filtros de categoria --

        private void FiltroTodos_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Todos");

        private void FiltroExatas_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Exatas");

        private void FiltroHumanas_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Humanas");

        private void FiltroTecnologia_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Tecnologia");

        private void AtivarCategoria(string categoria)
        {
            _categoriaAtiva = categoria;

            var corInativa = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2040"));
            var corAtiva = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A"));

            BtnTodos.Background = corInativa;
            BtnExatas.Background = corInativa;
            BtnHumanas.Background = corInativa;
            BtnTecnologia.Background = corInativa;

            switch (categoria)
            {
                case "Todos": BtnTodos.Background = corAtiva; break;
                case "Exatas": BtnExatas.Background = corAtiva; break;
                case "Humanas": BtnHumanas.Background = corAtiva; break;
                case "Tecnologia": BtnTecnologia.Background = corAtiva; break;
            }

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            string busca = TxtBusca.Text == PlaceholderBusca
                ? "" : TxtBusca.Text.ToLower().Trim();

            foreach (var card in new[] { CardCurso1, CardCurso2, CardCurso3, CardCurso4, CardCurso5 })
            {
                string categoria = card.Tag?.ToString() ?? "";
                bool passaCategoria = _categoriaAtiva == "Todos" || categoria == _categoriaAtiva;

                bool passaBusca = true;
                if (!string.IsNullOrEmpty(busca))
                {
                    var titulo = EncontrarTitulo(card);
                    passaBusca = titulo != null && titulo.ToLower().Contains(busca);
                }

                card.Visibility = (passaCategoria && passaBusca)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private string? EncontrarTitulo(Border card)
        {
            if (card.Child is Grid grid &&
                grid.Children.Count > 0 &&
                grid.Children[0] is StackPanel sp)
            {
                foreach (var child in sp.Children)
                {
                    if (child is TextBlock tb &&
                        tb.FontWeight == FontWeights.Bold &&
                        tb.FontSize == 15)
                        return tb.Text;
                }
            }
            return null;
        }

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            string nomeCurso = btn.Tag?.ToString() ?? "Curso";

            if (_alunoLogado == null)
            {
                MessageBox.Show("Voce precisa estar logado para se matricular.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Busca o curso no banco pelo titulo (cards estaticos do XAML)
            using var ctx = new LearnixDbContext();
            var cursoService = new CursoService(ctx);
            var cursos = cursoService.BuscarPorTermo(nomeCurso);
            var curso = cursos.FirstOrDefault(c => c.Titulo == nomeCurso);

            if (curso == null)
            {
                // Card estatico nao tem correspondente no banco. Tenta encontrar um curso da mesma categoria
                var categoria = btn.Tag?.ToString() ?? "";
                MessageBox.Show($"Curso "{nomeCurso}" ainda nao esta disponivel no banco.\n\nUse os cursos demo disponiveis em "Meus Cursos" após o seed.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var matriculaService = new MatriculaService(ctx);
            var matricula = matriculaService.CriarMatricula(_alunoLogado.Id, curso.Id);

            if (matricula == null)
            {
                MessageBox.Show($"Voce ja esta matriculado em "{nomeCurso}" ou houve um erro.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Matricula realizada com sucesso em: {nomeCurso}",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
