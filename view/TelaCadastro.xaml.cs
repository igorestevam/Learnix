using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.Controllers;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaCadastro : UserControl
    {
        public event RoutedEventHandler? SolicitarLogin;

        public TelaCadastro()
        {
            InitializeComponent();
        }

        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Password;
            string confirmar = txtConfirmarSenha.Password;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(confirmar))
            {
                MessageBox.Show("Preencha todos os campos.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (senha != confirmar)
            {
                MessageBox.Show("As senhas nao coincidem.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Persistencia real no banco de dados via CadastroController
            var dbContext = new LearnixDbContext();
            var cadastroService = new CadastroService(dbContext);
            var controller = new CadastroController(cadastroService);

            Aluno? alunoCriado = controller.CadastrarAluno(nome, email, senha);

            if (alunoCriado == null)
            {
                MessageBox.Show("E-mail ou matricula academica ja cadastrados. Tente outro e-mail.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(
                $"Cadastro realizado com sucesso!\n\nSua matricula academica e: {alunoCriado.MatriculaAcademica}\n\nUse-a (ou seu e-mail) e a senha para fazer login.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}
