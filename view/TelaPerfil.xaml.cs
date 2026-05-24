using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private bool _modoEdicao = false;
        private Aluno? _aluno;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            TxtNomePerfil.Text = aluno.Nome;
            TxtEditNome.Text = aluno.Nome;
            TxtEditEmail.Text = aluno.Email;
            TxtEmailPerfil.Text = aluno.Email;
            Sidebar.DefinirAluno(aluno.Nome);

            // Iniciais do avatar
            if (aluno.Nome.Length > 0)
            {
                var partes = aluno.Nome.Split(' ');
                TxtIniciais.Text = partes.Length >= 2
                    ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                    : aluno.Nome[0].ToString().ToUpper();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            _modoEdicao = true;

            var corEdicao = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#4E3A7A"));

            TxtEditNome.IsReadOnly = false;
            TxtEditEmail.IsReadOnly = false;

            TxtEditNome.Background = corEdicao;
            TxtEditEmail.Background = corEdicao;

            BtnEditar.Visibility = Visibility.Collapsed;
            BtnSalvar.Visibility = Visibility.Visible;

            TxtEditNome.Focus();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtEditNome.Text) ||
                string.IsNullOrWhiteSpace(TxtEditEmail.Text))
            {
                MessageBox.Show("Nome e e-mail são obrigatórios.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Persiste no banco de dados
            if (_aluno != null)
            {
                using var db = new LearnixDbContext();
                var aluno = db.Alunos.Find(_aluno.Id);
                if (aluno != null)
                {
                    aluno.Nome  = TxtEditNome.Text;
                    aluno.Email = TxtEditEmail.Text;
                    db.SaveChanges();
                    _aluno.Nome  = aluno.Nome;
                    _aluno.Email = aluno.Email;
                }
            }

            // Atualiza exibição
            TxtNomePerfil.Text = TxtEditNome.Text;
            TxtEmailPerfil.Text = TxtEditEmail.Text;
            Sidebar.DefinirAluno(TxtEditNome.Text);

            var partes = TxtEditNome.Text.Split(' ');
            TxtIniciais.Text = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                : TxtEditNome.Text[0].ToString().ToUpper();

            var corLeitura = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#3A2860"));

            TxtEditNome.IsReadOnly = true;
            TxtEditEmail.IsReadOnly = true;
            TxtEditNome.Background = corLeitura;
            TxtEditEmail.Background = corLeitura;

            BtnSalvar.Visibility = Visibility.Collapsed;
            BtnEditar.Visibility = Visibility.Visible;
            _modoEdicao = false;

            MessageBox.Show("Perfil atualizado com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
