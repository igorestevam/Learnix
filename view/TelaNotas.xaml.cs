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
            // A tela esta 100% pronta com avaliacoes estaticas em XAML.
            // Aqui apenas atualizamos o nome do aluno na sidebar.
            if (matricula?.Aluno != null)
            {
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);
            }
        }
    }
}
