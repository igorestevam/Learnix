using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
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

            CarregarResumo(aluno.Id);
        }

        private void CarregarResumo(int alunoId)
        {
            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == alunoId)
                .Include(m => m.Curso)
                .Include(m => m.Progresso)
                .ToList();

            TxtTotalCursos.Text = matriculas.Count.ToString();

            int totalHoras = matriculas.Sum(m => m.Curso?.CargaHoraria ?? 0);
            TxtHorasEstudadas.Text = $"{totalHoras}h";

            // Média usando RespostasAtividades igual à TelaNotas
            var matriculaIds = matriculas.Select(m => m.Id).ToList();
            var respostas = db.RespostasAtividades
                .Where(r => matriculaIds.Contains(r.MatriculaId))
                .ToList();

            var notasFinais = new List<decimal>();
            foreach (var m in matriculas)
            {
                bool avaliado = m.Status == StatusMatricula.Concluida ||
                                m.Status == StatusMatricula.Reprovada;
                if (!avaliado) continue;

                var resp = respostas
                    .Where(r => r.MatriculaId == m.Id)
                    .OrderBy(r => r.Id)
                    .ToList();
                if (!resp.Any()) continue;

                decimal nota1 = resp.ElementAtOrDefault(0)?.Nota ?? 0m;
                decimal nota2 = resp.ElementAtOrDefault(1)?.Nota ?? 0m;
                decimal nota3 = resp.ElementAtOrDefault(2)?.Nota ?? 0m;
                notasFinais.Add((nota1 + nota2 + nota3) / 3m);
            }

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
                var alunoDb = db.Alunos.Find(_aluno.Id);
                if (alunoDb != null)
                {
                    alunoDb.Nome = TxtEditNome.Text.Trim();
                    alunoDb.Email = TxtEditEmail.Text.Trim();
                    db.SaveChanges();
                    _aluno.Nome = alunoDb.Nome;
                    _aluno.Email = alunoDb.Email;
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