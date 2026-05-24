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

        public void DefinirMatricula(Matricula? matricula)
        {
            if (matricula == null) return;
            Sidebar.DefinirAluno(matricula.Aluno?.Nome ?? "Aluno");
            // Os dados de notas são exibidos via elementos estáticos do XAML.
            // Para binding dinâmico, adicione x:Name="PainelAvaliacoes" e
            // x:Name="TxtMediaGeral" no TelaNotas.xaml.
        }
    }
}
