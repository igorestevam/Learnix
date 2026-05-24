using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.Controllers;
using Learnix.data;
using Learnix.Repositorio;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private string _categoriaAtiva = "Todos";
        private const string PlaceholderBusca = "Buscar curso...";

        public TelaMenu()
        {
            InitializeComponent();
            CarregarCursos();
        }

        // ── Carregar cursos via CursoController ───────────────────────────────

        public void CarregarCursos(string termoBusca = "")
        {
            var controller = new CursoController(new CursoRepository(new LearnixDbContext()));
            var cursos = controller.BuscarCursos(termoBusca);
            ListaCursos.ItemsSource = cursos;
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
            CarregarCursos(TxtBusca.Text.Trim());
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

            // Recarrega com filtro de categoria aplicado via LINQ no DataTemplate
            // (o binding do ItemsSource já traz todos; CollectionView filtra localmente)
            AplicarFiltroCategoria();
        }

        private void AplicarFiltroCategoria()
        {
            if (ListaCursos.ItemsSource is System.Collections.Generic.List<Learnix.model.Curso> cursos)
            {
                var view = System.Windows.Data.CollectionViewSource.GetDefaultView(cursos);
                view.Filter = item =>
                {
                    if (_categoriaAtiva == "Todos") return true;
                    if (item is Learnix.model.Curso curso)
                        return curso.Categoria?.Nome == _categoriaAtiva;
                    return false;
                };
                view.Refresh();
            }
        }
    }
}
