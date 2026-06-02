using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.control;

namespace Learnix
{
    public partial class TelaEsqueceuSenha : UserControl
    {
        private readonly LoginController _loginController = new();

        public event RoutedEventHandler? SolicitarLogin;

        public TelaEsqueceuSenha()
        {
            InitializeComponent();
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Informe seu e-mail.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? senha = _loginController.RecuperarSenha(email);

            if (senha == null)
            {
                MessageBox.Show(
                    $"O e-mail '{email}' nao esta cadastrado no sistema.\n\nVerifique o e-mail digitado ou realize um novo cadastro.",
                    "E-mail nao encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(
                $"Conta encontrada!\n\nSua senha cadastrada e:\n\n{senha}\n\nUse-a para fazer login.",
                "Recuperacao de Senha", MessageBoxButton.OK, MessageBoxImage.Information);

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}
