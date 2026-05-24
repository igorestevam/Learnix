using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        public event RoutedEventHandler? SolicitarCadastro;
        public event RoutedEventHandler? SolicitarRecuperacaoSenha;

        // Passa o objeto Usuario autenticado após login bem-sucedido
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
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dbContext = new LearnixDbContext();
            var authService = new AuthService(dbContext);
            var controller = new LoginController(authService);

            Usuario usuarioAutenticado = authService.RealizarLogin(codigoAcesso, senha);
            if (usuarioAutenticado != null)
            {
                SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuarioAutenticado);
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
            => SolicitarCadastro?.Invoke(this, new RoutedEventArgs());

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
            => SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
    }
}
