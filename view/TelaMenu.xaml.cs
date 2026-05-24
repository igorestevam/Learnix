using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private string _categoriaAtiva = "Todos";
        private const string PlaceholderBusca = "Buscar curso...";

        public TelaMenu()
        {
            InitializeComponent();
        }

        // ── Busca ────────────────────────────────────────────────────────────

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

        // ── Filtros de categoria ─────────────────────────────────────────────

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
            var corAtiva   = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A"));

            BtnTodos.Background      = corInativa;
            BtnExatas.Background     = corInativa;
            BtnHumanas.Background    = corInativa;
            BtnTecnologia.Background = corInativa;

            switch (categoria)
            {
                case "Todos":      BtnTodos.Background      = corAtiva; break;
                case "Exatas":     BtnExatas.Background     = corAtiva; break;
                case "Humanas":    BtnHumanas.Background    = corAtiva; break;
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
            if (sender is Button btn)
            {
                string nomeCurso = btn.Tag?.ToString() ?? "Curso";
                MessageBox.Show($"Matrícula solicitada para: {nomeCurso}",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
