using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private readonly AlunoController _alunoController = new();
        private readonly MatriculaController _matriculaController = new();

        private Aluno? _aluno;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            TxtNomePerfil.Text = aluno.Nome;
            TxtEmailPerfil.Text = aluno.Email;
            TxtEditNome.Text = aluno.Nome;
            TxtEditEmail.Text = aluno.Email;
            TxtMatricula.Text = aluno.MatriculaAcademica;
            TxtMembroDesde.Text = aluno.DataCadastro.ToString("dd/MM/yyyy");
            Sidebar.DefinirAluno(aluno.Nome);

            var partes = aluno.Nome.Split(' ');
            TxtIniciais.Text = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                : aluno.Nome[0].ToString().ToUpper();

            CmbEstilo.ItemsSource = new[] { "Visual", "Auditivo", "Leitura/Escrita", "Cinestésico" };
            CmbRitmo.ItemsSource = new[] { "Intensivo", "Regular", "Flexível" };

            var perfil = _alunoController.ObterPerfil(aluno.Id);
            string estilo = perfil?.EstiloPredominante ?? "Não definido";
            string ritmo = perfil?.RitmoSugerido ?? "Regular";

            TxtEstilo.Text = estilo;
            TxtRitmo.Text = ritmo;
            CmbEstilo.SelectedItem = estilo;
            CmbRitmo.SelectedItem = ritmo;

            CarregarResumo(aluno.Id);
        }

        private void CarregarResumo(int alunoId)
        {
            var matriculas = _matriculaController.ListarPorAluno(alunoId);

            TxtTotalCursos.Text = matriculas.Count.ToString();
            TxtHorasEstudadas.Text = $"{matriculas.Sum(m => m.Curso?.CargaHoraria ?? 0)}h";

            var notasFinais = matriculas
                .Where(m => m.Avaliacoes?.Any() == true)
                .Select(m => m.Avaliacoes!.Average(a => a.Nota))
                .ToList();

            TxtMediaGeral.Text = notasFinais.Any()
                ? notasFinais.Average().ToString("0.0", new CultureInfo("pt-BR"))
                : "—";
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var corEdicao = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#4E3A7A"));
            TxtEditNome.IsReadOnly = false;
            TxtEditEmail.IsReadOnly = false;
            TxtEditNome.Background = corEdicao;
            TxtEditEmail.Background = corEdicao;

            TxtEstilo.Visibility = Visibility.Collapsed;
            TxtRitmo.Visibility = Visibility.Collapsed;
            CmbEstilo.Visibility = Visibility.Visible;
            CmbRitmo.Visibility = Visibility.Visible;

            BtnEditar.Visibility = Visibility.Collapsed;
            BtnSalvar.Visibility = Visibility.Visible;
            TxtEditNome.Focus();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = TxtEditNome.Text.Trim();
            string email = TxtEditEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Nome e e-mail são obrigatórios.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string estilo = CmbEstilo.SelectedItem?.ToString() ?? TxtEstilo.Text;
            string ritmo = CmbRitmo.SelectedItem?.ToString() ?? TxtRitmo.Text;

            if (_aluno != null)
            {
                _alunoController.AtualizarPerfil(_aluno.Id, nome, email);
                _alunoController.AtualizarPerfilAprendizagem(_aluno.Id, estilo, ritmo);
                _aluno.Nome = nome;
                _aluno.Email = email;
            }

            TxtNomePerfil.Text = nome;
            TxtEmailPerfil.Text = email;
            TxtEstilo.Text = estilo;
            TxtRitmo.Text = ritmo;
            Sidebar.DefinirAluno(nome);

            var partes = nome.Split(' ');
            TxtIniciais.Text = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                : nome[0].ToString().ToUpper();

            var corLeitura = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#3A2860"));
            TxtEditNome.IsReadOnly = true;
            TxtEditEmail.IsReadOnly = true;
            TxtEditNome.Background = corLeitura;
            TxtEditEmail.Background = corLeitura;

            TxtEstilo.Visibility = Visibility.Visible;
            TxtRitmo.Visibility = Visibility.Visible;
            CmbEstilo.Visibility = Visibility.Collapsed;
            CmbRitmo.Visibility = Visibility.Collapsed;

            BtnSalvar.Visibility = Visibility.Collapsed;
            BtnEditar.Visibility = Visibility.Visible;

            MessageBox.Show("Perfil atualizado com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
