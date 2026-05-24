using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private string _nomeAluno = "Aluno";
        private List<Matricula> _matriculas = new();

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _nomeAluno = aluno.Nome;
            _matriculas = aluno.HistoricoMatriculas ?? new List<Matricula>();
            Sidebar.DefinirAluno(aluno.Nome);
            CarregarCards();
        }

        private void CarregarCards()
        {
            // Gera os cards dinamicamente a partir das matrículas do aluno
            PainelCursos.Children.Clear();
            foreach (var matricula in _matriculas)
            {
                var card = CriarCardMatricula(matricula);
                PainelCursos.Children.Add(card);
            }
        }

        private UIElement CriarCardMatricula(Matricula matricula)
        {
            var border = new Border
            {
                Margin = new Thickness(0, 0, 0, 12),
                Tag = matricula
            };

            var stack = new StackPanel { Margin = new Thickness(12) };

            stack.Children.Add(new TextBlock { Text = matricula.Curso?.Titulo, FontSize = 16, FontWeight = System.Windows.FontWeights.Bold, Foreground = System.Windows.Media.Brushes.White });
            stack.Children.Add(new TextBlock { Text = $"Instrutor: {matricula.Curso?.Instrutor?.Nome}", Foreground = System.Windows.Media.Brushes.LightGray });
            stack.Children.Add(new TextBlock { Text = $"Categoria: {matricula.Curso?.Categoria?.Nome}", Foreground = System.Windows.Media.Brushes.LightGray });
            stack.Children.Add(new TextBlock { Text = $"Carga Horária: {matricula.Curso?.CargaHoraria}h", Foreground = System.Windows.Media.Brushes.LightGray });
            stack.Children.Add(new TextBlock { Text = $"Progresso: {matricula.Progresso?.PercentualConcluido}%", Foreground = System.Windows.Media.Brushes.LightGray });

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };

            var btnContinuar = new Button { Content = "Continuar", Tag = matricula, Margin = new Thickness(0, 0, 8, 0) };
            btnContinuar.Click += BtnContinuar_Click;

            bool concluido = matricula.Status == StatusMatricula.Concluida;
            var btnConcluir = new Button
            {
                Content = concluido ? "✔ Concluído" : "Concluir",
                Tag = matricula,
                IsEnabled = !concluido
            };
            btnConcluir.Click += BtnConcluir_Click;

            btnPanel.Children.Add(btnContinuar);
            btnPanel.Children.Add(btnConcluir);
            stack.Children.Add(btnPanel);
            border.Child = stack;

            return border;
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Matricula matricula)
            {
                var main = Application.Current.MainWindow as MainWindow;
                main?.MostrarAulas(matricula);
            }
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Matricula matricula)
            {
                // Registra a conclusão via ProgressoService
                var progressoService = new ProgressoService(new LearnixDbContext());
                // Conclui todas as aulas pendentes e emite certificado
                foreach (var modulo in matricula.Curso?.Modulos ?? new List<Modulo>())
                    foreach (var aula in modulo.Aulas)
                        progressoService.RegistrarConclusaoAula(matricula.Id, aula.Id);

                btn.IsEnabled = false;
                btn.Content = "✔ Concluído";

                string msg = "Parabéns! Você concluiu o curso \"" + matricula.Curso?.Titulo + "\"!\n\n"
                    + "Seu certificado já está disponível em \"Meus Certificados\".\n\n"
                    + "Deseja ver o certificado agora?";

                var resultado = MessageBox.Show(msg,
                    "Learnix", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (resultado == MessageBoxResult.Yes)
                {
                    var main = Application.Current.MainWindow as MainWindow;
                    main?.MostrarCertificados(_nomeAluno);
                }
            }
        }
    }
}
