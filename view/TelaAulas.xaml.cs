using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = "Aluno";

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirCurso(string nomeCurso, string professor,
            string categoria, string cargaHoraria, string descricao, string progresso)
        {
            TxtNomeCurso.Text    = nomeCurso;
            TxtProfessor.Text    = professor;
            TxtCategoria.Text    = categoria;
            TxtCargaHoraria.Text = cargaHoraria;
            TxtDescricao.Text    = descricao;
            TxtProgresso.Text    = progresso;
        }

        public void DefinirAluno(string nome)
        {
            _nomeAluno = nome;
            Sidebar.DefinirAluno(nome);
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos(_nomeAluno);
        }

        private void AulaCard_Click(object sender, MouseButtonEventArgs e)
        {
            // Clique no card inteiro também abre o player
            if (sender is Border card && card.Opacity >= 1.0)
            {
                var titulo = ObterTituloAula(card);
                if (titulo != null) AbrirPlayer(titulo);
            }
        }

        private void BtnAssistir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string titulo)
                AbrirPlayer(titulo);
        }

        private void AbrirPlayer(string tituloAula)
        {
            var player = new TelaPlayer();
            player.DefinirAula(tituloAula, TxtNomeCurso.Text, _nomeAluno);
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarTela(player, _nomeAluno);
        }

        private string ObterTituloAula(Border card)
        {
            // Busca o TextBlock de título (FontWeight Bold, FontSize 14) dentro do card
            if (card.Child is Grid grid)
                foreach (var col in grid.Children)
                    if (col is StackPanel sp)
                        foreach (var child in sp.Children)
                            if (child is TextBlock tb && tb.FontSize == 14)
                                return tb.Text;
            return null;
        }
    }
}
