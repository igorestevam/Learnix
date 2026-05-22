using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        public event RoutedEventHandler SolicitarCadastro;
        public event RoutedEventHandler SolicitarRecuperacaoSenha;

        public TelaLogin()
        {
            InitializeComponent();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Password;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: validação real (banco de dados / serviço)
            if (usuario == "admin" && senha == "1234")
            {
                MessageBox.Show("Login realizado com sucesso!",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navegar para tela principal aqui
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarCadastro?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
        }
    }
}