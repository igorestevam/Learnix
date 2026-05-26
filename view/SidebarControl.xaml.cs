using System;
using System.Windows;
using System.Windows.Controls;

namespace Learnix
{
    public partial class SidebarControl : UserControl
    {
        public event EventHandler SolicitarHome;
        public event EventHandler SolicitarBuscarCursos;
        public event EventHandler SolicitarNotas;
        public event EventHandler SolicitarMeusCursos;
        public event EventHandler SolicitarCertificados;
        public event EventHandler SolicitarPerfil;
        public event EventHandler SolicitarSair;

        // Mantido por compatibilidade com código antigo que usa SolicitarMenu
        public event EventHandler SolicitarMenu;

        public SidebarControl()
        {
            InitializeComponent();
        }

        public void DefinirAluno(string nome)
        {
            TxtNomeAluno.Text = nome;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            SolicitarHome?.Invoke(this, EventArgs.Empty);
        }

        private void BtnBuscarCursos_Click(object sender, RoutedEventArgs e)
            => SolicitarBuscarCursos?.Invoke(this, EventArgs.Empty);

        private void BtnNotas_Click(object sender, RoutedEventArgs e)
            => SolicitarNotas?.Invoke(this, EventArgs.Empty);

        private void BtnMeusCursos_Click(object sender, RoutedEventArgs e)
            => SolicitarMeusCursos?.Invoke(this, EventArgs.Empty);

        private void BtnCertificados_Click(object sender, RoutedEventArgs e)
            => SolicitarCertificados?.Invoke(this, EventArgs.Empty);

        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
            => SolicitarPerfil?.Invoke(this, EventArgs.Empty);

        private void BtnSair_Click(object sender, RoutedEventArgs e)
            => SolicitarSair?.Invoke(this, EventArgs.Empty);
    }
}