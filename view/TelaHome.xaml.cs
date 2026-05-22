using System;
using System.Globalization;
using System.Windows.Controls;

namespace Learnix
{
    public partial class TelaHome : UserControl
    {
        public TelaHome()
        {
            InitializeComponent();
            TxtData.Text = DateTime.Now.ToString(
                "dddd, dd 'de' MMMM 'de' yyyy",
                new CultureInfo("pt-BR"));
        }

        public void DefinirAluno(string nome)
        {
            TxtNomeAluno.Text = nome;
            Sidebar.DefinirAluno(nome);
        }
    }
}
