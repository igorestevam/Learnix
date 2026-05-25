using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.data;

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
                MessageBox.Show("Informe seu e-mail.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica se o e-mail esta cadastrado no banco (Aluno ou Instrutor)
            using var ctx = new LearnixDbContext();
            bool emailCadastrado = ctx.Usuarios.Any(u => u.Email == email);

            if (!emailCadastrado)
            {
                MessageBox.Show(
                    $"O e-mail '{email}' nao esta cadastrado no sistema.\n\nVerifique o e-mail digitado ou realize um novo cadastro.",
                    "E-mail nao encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // E-mail encontrado — exibe a senha cadastrada (comportamento academico)
            // Em producao, aqui seria disparado um e-mail real de recuperacao
            var usuario = ctx.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null)
            {
                MessageBox.Show(
                    $"Conta encontrada!\n\nSua senha cadastrada e:\n\n{usuario.Senha}\n\nUse-a para fazer login.",
                    "Recuperacao de Senha", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}