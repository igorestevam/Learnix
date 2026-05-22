using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private bool _modoEdicao = false;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(string nome, string email = "")
        {
            TxtNomePerfil.Text  = nome;
            TxtEditNome.Text    = nome;
            Sidebar.DefinirAluno(nome);

            // Iniciais do avatar
            if (nome.Length > 0)
            {
                var partes = nome.Split(' ');
                TxtIniciais.Text = partes.Length >= 2
                    ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                    : nome[0].ToString().ToUpper();
            }

            if (!string.IsNullOrEmpty(email))
            {
                TxtEmailPerfil.Text = email;
                TxtEditEmail.Text   = email;
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            _modoEdicao = true;

            // Habilita os campos editáveis
            var corEdicao = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#4E3A7A"));

            TxtEditNome.IsReadOnly      = false;
            TxtEditEmail.IsReadOnly     = false;
            TxtEditTelefone.IsReadOnly  = false;
            TxtEditNascimento.IsReadOnly = false;

            TxtEditNome.Background      = corEdicao;
            TxtEditEmail.Background     = corEdicao;
            TxtEditTelefone.Background  = corEdicao;
            TxtEditNascimento.Background = corEdicao;

            BtnEditar.Visibility = Visibility.Collapsed;
            BtnSalvar.Visibility = Visibility.Visible;

            TxtEditNome.Focus();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(TxtEditNome.Text) ||
                string.IsNullOrWhiteSpace(TxtEditEmail.Text))
            {
                MessageBox.Show("Nome e e-mail s&#xE3;o obrigat&#xF3;rios.",
                    "Aten&#xE7;&#xE3;o", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atualiza exibição
            TxtNomePerfil.Text  = TxtEditNome.Text;
            TxtEmailPerfil.Text = TxtEditEmail.Text;
            Sidebar.DefinirAluno(TxtEditNome.Text);

            // Atualiza iniciais do avatar
            var partes = TxtEditNome.Text.Split(' ');
            TxtIniciais.Text = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                : TxtEditNome.Text[0].ToString().ToUpper();

            // Volta para modo leitura
            var corLeitura = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#3A2860"));

            TxtEditNome.IsReadOnly      = true;
            TxtEditEmail.IsReadOnly     = true;
            TxtEditTelefone.IsReadOnly  = true;
            TxtEditNascimento.IsReadOnly = true;

            TxtEditNome.Background      = corLeitura;
            TxtEditEmail.Background     = corLeitura;
            TxtEditTelefone.Background  = corLeitura;
            TxtEditNascimento.Background = corLeitura;

            BtnSalvar.Visibility = Visibility.Collapsed;
            BtnEditar.Visibility = Visibility.Visible;

            _modoEdicao = false;

            // TODO: persistir no banco de dados
            MessageBox.Show("Perfil atualizado com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
