using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaEsqueceuSenha : UserControl
    {
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
                MessageBox.Show("Informe seu e-mail.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: integrar com serviço de e-mail / back-end
            MessageBox.Show($"Se o e-mail '{email}' estiver cadastrado, você receberá as instruções em breve.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}