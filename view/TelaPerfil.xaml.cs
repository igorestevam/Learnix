using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.view
{
    public partial class TelaPerfil : UserControl
    {
        private Aluno? _aluno;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;

            // Dados básicos
            TxtNomePerfil.Text = aluno.Nome;
            TxtEmailPerfil.Text = aluno.Email;
            TxtEditNome.Text = aluno.Nome;
            TxtEditEmail.Text = aluno.Email;
            TxtMatricula.Text = aluno.MatriculaAcademica;
            TxtMembroDesde.Text = aluno.DataCadastro.ToString("dd/MM/yyyy");
            Sidebar.DefinirAluno(aluno.Nome);

            // Iniciais do avatar
            var partes = aluno.Nome.Split(' ');
            TxtIniciais.Text = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                : aluno.Nome[0].ToString().ToUpper();

            CarregarResumo(aluno.Id);
        }

        private void CarregarResumo(int alunoId)
        {
            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == alunoId)
                .Include(m => m.Curso)
                .Include(m => m.Avaliacoes)
                .Include(m => m.Progresso)
                .ToList();

            TxtTotalCursos.Text = matriculas.Count.ToString();

            var todasNotas = matriculas.SelectMany(m => m.Avaliacoes).ToList();
            TxtMediaGeral.Text = todasNotas.Any()
                ? todasNotas.Average(a => a.Nota).ToString("0.0", new CultureInfo("pt-BR"))
                : "—";

            int totalHoras = matriculas.Sum(m => m.Curso?.CargaHoraria ?? 0);
            TxtHorasEstudadas.Text = $"{totalHoras}h";

            int aprovados = matriculas.Count(m =>
            {
                var notas = m.Avaliacoes?.ToList();
                if (notas == null || !notas.Any()) return false;
                return notas.Average(a => a.Nota) >= 7.0;
            });
            TxtAprovados.Text = aprovados.ToString();
        }

        private void CarregarDados()
        {
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

            if (_aluno != null)
            {
                using var db = new LearnixDbContext();
                var aluno = db.Alunos.Find(_aluno.Id);
                if (aluno != null)
                {
                    aluno.Nome = TxtEditNome.Text.Trim();
                    aluno.Email = TxtEditEmail.Text.Trim();
                    db.SaveChanges();
                    _aluno.Nome = aluno.Nome;
                    _aluno.Email = aluno.Email;
                }
            }

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

            MessageBox.Show("Perfil atualizado com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}