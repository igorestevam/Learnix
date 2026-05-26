using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class TelaCadastro : UserControl
    {
        public event RoutedEventHandler? SolicitarLogin;

        private bool _isInstrutor = false;

        public TelaCadastro()
        {
            InitializeComponent();
        }

        private void TipoAluno_Click(object sender, MouseButtonEventArgs e)
        {
            _isInstrutor = false;
            BtnTipoAluno.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A"));
            BtnTipoInstrutor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2060"));
            PainelEspecialidade.Visibility = Visibility.Collapsed;
        }

        private void TipoInstrutor_Click(object sender, MouseButtonEventArgs e)
        {
            _isInstrutor = true;
            BtnTipoInstrutor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A"));
            BtnTipoAluno.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2060"));
            PainelEspecialidade.Visibility = Visibility.Visible;
        }

        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Password;
            string confirmar = txtConfirmarSenha.Password;
            string especialidade = txtEspecialidade.Text.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(confirmar))
            {
                MessageBox.Show("Preencha todos os campos.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isInstrutor && string.IsNullOrEmpty(especialidade))
            {
                MessageBox.Show("Informe a especialidade do instrutor.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (senha != confirmar)
            {
                MessageBox.Show("As senhas nao coincidem.", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new LearnixDbContext();

            bool emailExiste = db.Usuarios.Any(u => u.Email == email);
            if (emailExiste)
            {
                MessageBox.Show("E-mail já cadastrado. Tente outro.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isInstrutor)
            {
                var instrutor = new Instrutor
                {
                    Nome = nome,
                    Email = email,
                    Senha = senha,
                    Especialidade = especialidade,
                    Biografia = string.Empty,
                    DataCadastro = System.DateTime.Now,
                };
                db.Instrutores.Add(instrutor);
                db.SaveChanges();

                MessageBox.Show(
                    $"Cadastro de instrutor realizado com sucesso!\n\nUse seu e-mail e senha para fazer login.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string matricula = (email.Contains('@') ? email.Split('@')[0] : email).ToUpper();

                var perfil = new PerfilDeAprendizagem
                {
                    EstiloPredominante = "Não definido",
                    RitmoSugerido = "Regular",
                };
                db.PerfisDeAprendizagem.Add(perfil);
                db.SaveChanges();

                var novoAluno = new Aluno
                {
                    Nome = nome,
                    Email = email,
                    Senha = senha,
                    MatriculaAcademica = matricula,
                    DataCadastro = System.DateTime.Now,
                    PerfilDeAprendizagemId = perfil.Id,
                };
                db.Alunos.Add(novoAluno);
                db.SaveChanges();

                MessageBox.Show(
                    $"Cadastro realizado com sucesso!\n\nSua matrícula é: {matricula}\n\nUse-a ou seu e-mail para fazer login.",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}