using System;
using System.Windows;
using System.Windows.Controls;

namespace Learnix
{
    public partial class SidebarInstrutor : UserControl
    {
        public event EventHandler? SolicitarHome;
        public event EventHandler? SolicitarMeusCursos;
        public event EventHandler? SolicitarEscolherCurso;
        public event EventHandler? SolicitarLancarNotas;
        public event EventHandler? SolicitarSair;

        public SidebarInstrutor()
        {
            InitializeComponent();
        }

        public void DefinirInstrutor(string nome)
        {
            TxtNomeInstrutor.Text = nome;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
            => SolicitarHome?.Invoke(this, EventArgs.Empty);

        private void BtnMeusCursos_Click(object sender, RoutedEventArgs e)
            => SolicitarMeusCursos?.Invoke(this, EventArgs.Empty);

        private void BtnEscolherCurso_Click(object sender, RoutedEventArgs e)
            => SolicitarEscolherCurso?.Invoke(this, EventArgs.Empty);

        private void BtnLancarNotas_Click(object sender, RoutedEventArgs e)
            => SolicitarLancarNotas?.Invoke(this, EventArgs.Empty);

        private void BtnSair_Click(object sender, RoutedEventArgs e)
            => SolicitarSair?.Invoke(this, EventArgs.Empty);
    }
}