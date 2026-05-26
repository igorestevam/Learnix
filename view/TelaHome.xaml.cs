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

        // Sobrecarga antiga mantida por compatibilidade
        public void DefinirAluno(string nome)
        {
            TxtNomeAluno.Text = nome;
            Sidebar.DefinirAluno(nome);
        }

        private void CarregarDados()
        {
            if (_aluno == null) return;

            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == _aluno.Id)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso).ThenInclude(c => c.Categoria)
                .Include(m => m.Progresso)
                .Include(m => m.Avaliacoes)
                .ToList();

            // Cards de resumo
            int cursosAtivos = matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            TxtCursosAtivos.Text = cursosAtivos.ToString();

            var todasNotas = matriculas.SelectMany(m => m.Avaliacoes).ToList();
            if (todasNotas.Any())
            {
                double media = todasNotas.Average(a => a.Nota);
                TxtMediaGeral.Text = media.ToString("0.0", new CultureInfo("pt-BR"));
            }
            else
            {
                TxtMediaGeral.Text = "—";
            }

            // Pendentes = matrículas ativas sem 100% de progresso
            int pendentes = matriculas.Count(m =>
                m.Status == StatusMatricula.Ativa &&
                (m.Progresso == null || m.Progresso.PercentualConcluido < 100));
            TxtPendentes.Text = pendentes.ToString();

            // Lista "Continuar estudando" — só cursos ativos e não 100%
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

            // Próximas atividades — avaliações futuras
            var atividades = matriculas
                .SelectMany(m => m.Avaliacoes.Select(a => new { a, m }))
                .Where(x => x.a.DataRealizacao >= DateTime.Today)
                .OrderBy(x => x.a.DataRealizacao)
                .Take(5)
                .ToList();

            if (atividades.Count == 0)
            {
                PainelSemAtividades.Visibility = Visibility.Visible;
                ListaAtividades.Visibility = Visibility.Collapsed;
            }
            else
            {
                PainelSemAtividades.Visibility = Visibility.Collapsed;
                ListaAtividades.Visibility = Visibility.Visible;

                ListaAtividades.ItemsSource = atividades.Select(x => new HomeAtividadeVM
                {
                    Dia = x.a.DataRealizacao.Day.ToString(),
                    Titulo = $"Entrega: {x.a.Titulo}",
                    Subtitulo = $"{x.m.Curso?.Titulo}  •  {x.a.DataRealizacao:dd/MM/yyyy}",
                }).ToList();
            }
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