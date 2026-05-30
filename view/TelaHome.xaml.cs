using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaHome : UserControl
    {
        private Aluno? _aluno;
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

            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == _aluno.Id)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso).ThenInclude(c => c.Categoria)
                .Include(m => m.Progresso)
                .Include(m => m.Avaliacoes)
                .ToList();

            int cursosAtivos = matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            TxtCursosAtivos.Text = cursosAtivos.ToString();

            // Popula ComboBox com todos os cursos do aluno
            _matriculasVM = matriculas.Select(m => new MatriculaItemVM
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

            // Força atualização da média para o primeiro item
            CmbCursos_SelectionChanged(CmbCursos, null!);

            // Lista "Continuar estudando"
            var emAndamento = matriculas
                .Where(m => m.Status == StatusMatricula.Ativa &&
                            (m.Progresso == null || m.Progresso.PercentualConcluido < 100))
                .ToList();

            if (emAndamento.Count == 0)
            {
                PainelSemCursos.Visibility = Visibility.Visible;
                ListaCursosHome.Visibility = Visibility.Collapsed;
            }
            else
            {
                PainelSemCursos.Visibility = Visibility.Collapsed;
                ListaCursosHome.Visibility = Visibility.Visible;

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

            // Próximas atividades
            var cursoIds = matriculas
                .Where(m => m.Status == StatusMatricula.Ativa)
                .Select(m => m.CursoId).ToList();

            var respostasDoAluno = db.RespostasAtividades
                .Where(r => matriculas.Select(m => m.Id).Contains(r.MatriculaId))
                .Select(r => r.AtividadeCursoId)
                .ToHashSet();

            var atividadesPendentes = db.AtividadesCursos
                .Include(a => a.Curso)
                .Where(a => cursoIds.Contains(a.CursoId) && !respostasDoAluno.Contains(a.Id))
                .ToList();

            TxtPendentes.Text = atividadesPendentes.Count.ToString();

            if (atividadesPendentes.Count == 0)
            {
                PainelSemAtividades.Visibility = Visibility.Visible;
                ListaAtividades.Visibility = Visibility.Collapsed;
            }
            else
            {
                PainelSemAtividades.Visibility = Visibility.Collapsed;
                ListaAtividades.Visibility = Visibility.Visible;

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

            using var db = new LearnixDbContext();

            var matricula = db.Matriculas
                .FirstOrDefault(m => m.Id == vm.MatriculaId);

            if (matricula == null) { TxtMediaGeral.Text = "—"; return; }

            bool avaliado = matricula.Status == StatusMatricula.Concluida ||
                            matricula.Status == StatusMatricula.Reprovada;

            if (!avaliado) { TxtMediaGeral.Text = "—"; return; }

            var respostas = db.RespostasAtividades
                .Where(r => r.MatriculaId == vm.MatriculaId)
                .OrderBy(r => r.Id)
                .ToList();

            if (!respostas.Any()) { TxtMediaGeral.Text = "—"; return; }

            decimal nota1 = respostas.ElementAtOrDefault(0)?.Nota ?? 0m;
            decimal nota2 = respostas.ElementAtOrDefault(1)?.Nota ?? 0m;
            decimal nota3 = respostas.ElementAtOrDefault(2)?.Nota ?? 0m;
            decimal media = (nota1 + nota2 + nota3) / 3m;

            TxtMediaGeral.Text = media.ToString("0.0", new CultureInfo("pt-BR"));
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int matriculaId)
            {
                using var db = new LearnixDbContext();
                var matricula = db.Matriculas
                    .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
                    .Include(m => m.Progresso)
                    .FirstOrDefault(m => m.Id == matriculaId);

                if (matricula != null)
                {
                    var main = Application.Current.MainWindow as MainWindow;
                    main?.MostrarAulas(matricula);
                }
            }
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