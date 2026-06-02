using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaHome : UserControl
    {
        private readonly MatriculaController _matriculaController = new();
        private readonly AvaliacaoController _avaliacaoController = new();

        private Aluno? _aluno;
        private List<Matricula> _matriculas = new();
        private List<MatriculaItemVM> _matriculasVM = new();
        private bool _carregando = false;

        public TelaHome()
        {
            InitializeComponent();
            TxtData.Text = DateTime.Now.ToString(
                "dddd, dd 'de' MMMM 'de' yyyy",
                new CultureInfo("pt-BR"));
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            TxtNomeAluno.Text = aluno.Nome;
            Sidebar.DefinirAluno(aluno.Nome);
            CarregarDados();
        }

        public void DefinirAluno(string nome)
        {
            TxtNomeAluno.Text = nome;
            Sidebar.DefinirAluno(nome);
        }

        private void CarregarDados()
        {
            if (_aluno == null) return;

            _carregando = true;

            _matriculas = _matriculaController.ListarPorAluno(_aluno.Id);

            int cursosAtivos = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            TxtCursosAtivos.Text = cursosAtivos.ToString();

            _matriculasVM = _matriculas.Select(m => new MatriculaItemVM
            {
                MatriculaId = m.Id,
                Titulo = m.Curso?.Titulo ?? "Curso",
            }).ToList();

            CmbCursos.ItemsSource = _matriculasVM;
            CmbCursos.DisplayMemberPath = "Titulo";
            CmbCursos.SelectedValuePath = "MatriculaId";

            if (CmbCursos.Items.Count > 0)
                CmbCursos.SelectedIndex = 0;

            _carregando = false;

            CmbCursos_SelectionChanged(CmbCursos, null!);

            var emAndamento = _matriculas
                .Where(m => m.Status == StatusMatricula.Ativa &&
                            (m.Progresso == null || m.Progresso.PercentualConcluido < 100))
                .ToList();

            PainelSemCursos.Visibility = emAndamento.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaCursosHome.Visibility = emAndamento.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (emAndamento.Count > 0)
            {
                ListaCursosHome.ItemsSource = emAndamento.Select(m =>
                {
                    double pct = m.Progresso?.PercentualConcluido ?? 0;
                    string categoria = m.Curso?.Categoria?.Nome ?? "";
                    return new HomeCursoVM
                    {
                        MatriculaId = m.Id,
                        TituloCurso = m.Curso?.Titulo ?? "Curso",
                        SubtituloInstrutor = $"Prof. {m.Curso?.Instrutor?.Nome}  •  {categoria}",
                        PercentualTexto = $"{pct:0}% concluído",
                        LarguraBarra = Math.Min(pct / 100.0 * 300, 300),
                    };
                }).ToList();
            }

            var matriculaIds = _matriculas.Select(m => m.Id).ToList();
            var cursoIds = _matriculas
                .Where(m => m.Status == StatusMatricula.Ativa)
                .Select(m => m.CursoId).ToList();

            var atividadesPendentes = _avaliacaoController.ListarAtividadesPendentes(matriculaIds, cursoIds);

            TxtPendentes.Text = atividadesPendentes.Count.ToString();

            PainelSemAtividades.Visibility = atividadesPendentes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaAtividades.Visibility = atividadesPendentes.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (atividadesPendentes.Count > 0)
            {
                ListaAtividades.ItemsSource = atividadesPendentes.Select(a => new HomeAtividadeVM
                {
                    Dia = "📝",
                    Titulo = a.Pergunta.Length > 50 ? a.Pergunta[..50] + "..." : a.Pergunta,
                    Subtitulo = $"Curso: {a.Curso?.Titulo}",
                }).ToList();
            }
        }

        private void CmbCursos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_carregando) return;
            if (_aluno == null || CmbCursos.SelectedItem is not MatriculaItemVM vm) return;

            var matricula = _matriculas.FirstOrDefault(m => m.Id == vm.MatriculaId);
            if (matricula == null) { TxtMediaGeral.Text = "—"; return; }

            var avaliacoes = matricula.Avaliacoes?.OrderBy(a => a.Titulo).ToList();
            if (avaliacoes == null || !avaliacoes.Any()) { TxtMediaGeral.Text = "—"; return; }

            double media = avaliacoes.Average(a => a.Nota);
            TxtMediaGeral.Text = media.ToString("0.0", new CultureInfo("pt-BR"));
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            var matricula = _matriculaController.BuscarCompleta(matriculaId);
            if (matricula == null) return;

            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarAulas(matricula);
        }
    }

    public class MatriculaItemVM
    {
        public int MatriculaId { get; set; }
        public string Titulo { get; set; } = "";
    }

    public class HomeCursoVM
    {
        public int MatriculaId { get; set; }
        public string TituloCurso { get; set; } = "";
        public string SubtituloInstrutor { get; set; } = "";
        public string PercentualTexto { get; set; } = "";
        public double LarguraBarra { get; set; }
    }

    public class HomeAtividadeVM
    {
        public string Dia { get; set; } = "";
        public string Titulo { get; set; } = "";
        public string Subtitulo { get; set; } = "";
    }
}
