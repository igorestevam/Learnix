using System.Windows;
using System.Windows.Controls;

namespace Learnix
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MostrarLogin();
        }

        private void MostrarLogin()
        {
            var tela = new TelaLogin();
            tela.SolicitarCadastro += (s, e) => MostrarCadastro();
            tela.SolicitarRecuperacaoSenha += (s, e) => MostrarEsqueceuSenha();
            conteudoPrincipal.Content = tela;
        }

        private void MostrarCadastro()
        {
            var tela = new TelaCadastro();
            tela.SolicitarLogin += (s, e) => MostrarLogin();
            conteudoPrincipal.Content = tela;
        }

        private void MostrarEsqueceuSenha()
        {
            var tela = new TelaEsqueceuSenha();
            tela.SolicitarLogin += (s, e) => MostrarLogin();
            conteudoPrincipal.Content = tela;
        }
    }
}