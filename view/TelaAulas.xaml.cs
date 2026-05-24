using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.model;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = "Aluno";
        private Matricula? _matricula;

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
            // Os módulos e aulas são exibidos via cards estáticos no XAML.
            // Para tornar dinâmico no futuro, adicione um ItemsControl x:Name="PainelModulos" no XAML.
        }

        // ── Handlers dos cards de aula estáticos do XAML ─────────────────────

        private void AulaCard_Click(object sender, MouseButtonEventArgs e)
        {
            // Card clicado: navega para o player com a matrícula atual
            if (_matricula != null)
            {
                var player = new TelaPlayer();
                // Usa a primeira aula disponível na matrícula como fallback
                var aula = _matricula.Curso?.Modulos?.SelectMany(m => m.Aulas).FirstOrDefault();
                if (aula != null)
                    player.DefinirAula(aula, _matricula, _nomeAluno);
                (Application.Current.MainWindow as MainWindow)?.MostrarTela(player, _nomeAluno);
            }
        }

        private void BtnAssistir_Click(object sender, RoutedEventArgs e)
        {
            if (_matricula != null)
            {
                var player = new TelaPlayer();
                var aula = _matricula.Curso?.Modulos?.SelectMany(m => m.Aulas).FirstOrDefault();
                if (aula != null)
                    player.DefinirAula(aula, _matricula, _nomeAluno);
                (Application.Current.MainWindow as MainWindow)?.MostrarTela(player, _nomeAluno);
            }
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos(_nomeAluno);
        }
    }
}
