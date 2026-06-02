using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        private readonly LoginController _loginController = new();

        public event RoutedEventHandler? SolicitarCadastro;
        public event RoutedEventHandler? SolicitarRecuperacaoSenha;

        public delegate void HomeHandler(object sender, RoutedEventArgs e, Usuario usuario);
        public event HomeHandler? SolicitarHome;

        public TelaLogin()
        {
            InitializeComponent();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string codigoAcesso = txtUsuario.Text.Trim();
            string senha = txtSenha.Password;

            if (string.IsNullOrEmpty(codigoAcesso) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.",
                    "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Usuario? usuarioAutenticado = _loginController.RealizarLogin(codigoAcesso, senha);

            if (usuarioAutenticado != null)
            {
                SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuarioAutenticado);
            }
            else
            {
                MessageBox.Show("Usuario ou senha invalidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
            => SolicitarCadastro?.Invoke(this, new RoutedEventArgs());

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
            => SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
    }
}
