using System.Windows.Controls;
using Learnix.model;

namespace Learnix
{
    public partial class TelaNotas : UserControl
    {
        public TelaNotas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula)
        {
            Sidebar.DefinirAluno(matricula.Aluno?.Nome ?? "Aluno");

            // Limpa linhas anteriores
            PainelAvaliacoes.Children.Clear();

            // Popula cada avaliação
            foreach (var av in matricula.Avaliacoes)
            {
                var linha = new TextBlock
                {
                    Text = $"{av.Titulo}: {av.Nota:0.0}",
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new System.Windows.Thickness(0, 4, 0, 4)
                };
                PainelAvaliacoes.Children.Add(linha);
            }

            // Exibe a média geral
            TxtMediaGeral.Text = matricula.NotaFinal.ToString("0.0");
        }
    }
}
