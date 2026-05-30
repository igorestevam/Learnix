using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Learnix.Services;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = string.Empty;
        private Matricula? _matricula;
        private List<Aula> _aulas = new();
        private int _aulaAtualIndex = 0;

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula, string nomeAluno)
        {
            _nomeAluno = string.IsNullOrWhiteSpace(nomeAluno)
                ? (matricula?.Aluno?.Nome ?? "Aluno")
                : nomeAluno;

            using var db = new LearnixDbContext();
            _matricula = db.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Progresso)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso).ThenInclude(c => c.Categoria)
                .Include(m => m.Curso).ThenInclude(c => c.Modulos)
                    .ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matricula.Id) ?? matricula;

            Sidebar?.DefinirAluno(_nomeAluno);

            _aulas = _matricula.Curso?.Modulos?
                .OrderBy(m => m.Ordem)
                .SelectMany(m => m.Aulas.OrderBy(a => a.Ordem))
                .ToList() ?? new List<Aula>();

            var aulasConcluidas = db.AulasConcluidas
                .Where(ac => ac.MatriculaId == _matricula.Id)
                .Select(ac => ac.AulaId)
                .ToHashSet();

            _aulaAtualIndex = _aulas.FindIndex(a => !aulasConcluidas.Contains(a.Id));
            if (_aulaAtualIndex < 0) _aulaAtualIndex = _aulas.Count - 1;

            PopularDadosCurso();
            RenderizarAulas(aulasConcluidas);
            AtualizarNavegacao();
        }

        public void DefinirMatricula(Matricula matricula)
            => DefinirMatricula(matricula, matricula?.Aluno?.Nome ?? "Aluno");

        private void PopularDadosCurso()
        {
            if (_matricula?.Curso == null) return;
            TxtNomeCurso.Text = _matricula.Curso.Titulo;
            TxtProfessor.Text = _matricula.Curso.Instrutor != null
                                    ? $"Prof. {_matricula.Curso.Instrutor.Nome}" : "";
            double pct = _matricula?.Progresso?.PercentualConcluido ?? 0;
            TxtProgresso.Text = $"{(int)Math.Round(pct)}%";
        }

        private void RenderizarAulas(HashSet<int> aulasConcluidas)
        {
            PainelAulas.Children.Clear();

            string moduloAtual = "";
            for (int i = 0; i < _aulas.Count; i++)
            {
                var aula = _aulas[i];
                var modulo = _matricula?.Curso?.Modulos?
                    .FirstOrDefault(m => m.Aulas.Any(a => a.Id == aula.Id));

                if (modulo != null && modulo.Titulo != moduloAtual)
                {
                    moduloAtual = modulo.Titulo;
                    PainelAulas.Children.Add(new TextBlock
                    {
                        Text = modulo.Titulo,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold,
                        FontFamily = new FontFamily("Segoe UI"),
                        Margin = new Thickness(0, 12, 0, 8),
                    });
                }

                bool concluida = aulasConcluidas.Contains(aula.Id);
                bool eAtual = i == _aulaAtualIndex;
                bool bloqueada = !concluida && i > _aulaAtualIndex;

                PainelAulas.Children.Add(CriarCardAula(aula, i, concluida, eAtual, bloqueada));
            }
        }

        private Border CriarCardAula(Aula aula, int index, bool concluida, bool eAtual, bool bloqueada)
        {
            var card = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2040")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20, 16, 20, 16),
                Margin = new Thickness(0, 0, 0, 10),
                Cursor = bloqueada ? Cursors.Arrow : Cursors.Hand,
                Opacity = bloqueada ? 0.5 : 1.0,
            };

            if (eAtual)
            {
                card.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7E6BAC"));
                card.BorderThickness = new Thickness(2);
            }

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            string iconeTexto = concluida ? "✔" : eAtual ? "▶" : "🔒";
            string iconeBg = concluida ? "#1B5E20" : eAtual ? "#4E3A7A" : "#3A2860";
            string iconeFg = concluida ? "#A5D6A7" : "White";

            var iconeBorder = new Border
            {
                Width = 44,
                Height = 44,
                CornerRadius = new CornerRadius(22),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(iconeBg)),
                Margin = new Thickness(0, 0, 16, 0),
            };
            iconeBorder.Child = new TextBlock
            {
                Text = iconeTexto,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(iconeFg)),
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(iconeBorder, 0);

            string statusTexto = concluida ? "Assistida" : eAtual ? "Em andamento" : "Bloqueada";
            string statusCor = concluida ? "#A5D6A7" : eAtual ? "#FFCC80" : "#9E8FC0";

            var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            info.Children.Add(new TextBlock
            {
                Text = $"Aula {index + 1:D2} — {aula.Titulo}",
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Segoe UI"),
            });
            var subInfo = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            subInfo.Children.Add(new TextBlock
            {
                Text = $"🕑 {aula.Duracao.Minutes} min",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                FontSize = 12,
                FontFamily = new FontFamily("Segoe UI"),
                Margin = new Thickness(0, 0, 16, 0),
            });
            subInfo.Children.Add(new TextBlock
            {
                Text = statusTexto,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(statusCor)),
                FontSize = 12,
                FontFamily = new FontFamily("Segoe UI"),
            });
            info.Children.Add(subInfo);
            Grid.SetColumn(info, 1);

            grid.Children.Add(iconeBorder);
            grid.Children.Add(info);

            if (!bloqueada)
            {
                var btn = new Button
                {
                    Content = concluida ? "▶ Rever" : eAtual ? "▶ Continuar" : "▶ Assistir",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A")),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(14, 8, 14, 8),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 13,
                    Cursor = Cursors.Hand,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = index,
                };
                btn.Click += (s, e) =>
                {
                    _aulaAtualIndex = index;
                    AtualizarNavegacao();
                    RenderizarAulasSemRecarregar();

                    // Abre o player com a aula selecionada
                    var main = Application.Current.MainWindow as MainWindow;
                    main?.MostrarPlayer(_matricula!, aula);
                };
                Grid.SetColumn(btn, 2);
                grid.Children.Add(btn);
            }
            else
            {
                var bloqBorder = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2860")),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(14, 8, 14, 8),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                bloqBorder.Child = new TextBlock
                {
                    Text = "🔒 Bloqueada",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 13,
                };
                Grid.SetColumn(bloqBorder, 2);
                grid.Children.Add(bloqBorder);
            }

            card.Child = grid;
            return card;
        }

        private void RenderizarAulasSemRecarregar()
        {
            using var db = new LearnixDbContext();
            var aulasConcluidas = db.AulasConcluidas
                .Where(ac => ac.MatriculaId == _matricula!.Id)
                .Select(ac => ac.AulaId)
                .ToHashSet();
            RenderizarAulas(aulasConcluidas);
        }

        private void AtualizarNavegacao()
        {
            if (_aulas.Count == 0) return;

            var aulaAtual = _aulas[_aulaAtualIndex];
            TxtAulaAtual.Text = $"Aula atual: {_aulaAtualIndex + 1:D2} — {aulaAtual.Titulo}";
            TxtContadorAulas.Text = $"Aula {_aulaAtualIndex + 1} de {_aulas.Count}";

            BtnAnterior.IsEnabled = _aulaAtualIndex > 0;
            BtnProxima.IsEnabled = _aulaAtualIndex < _aulas.Count - 1;
        }

        private void BtnProxima_Click(object sender, RoutedEventArgs e)
        {
            if (_matricula == null || _aulaAtualIndex >= _aulas.Count - 1) return;

            var aulaAtual = _aulas[_aulaAtualIndex];
            using var db = new LearnixDbContext();

            bool jaRegistrada = db.AulasConcluidas
                .Any(ac => ac.MatriculaId == _matricula.Id && ac.AulaId == aulaAtual.Id);

            if (!jaRegistrada)
            {
                var service = new ProgressoService(db);
                service.RegistrarConclusaoAula(_matricula.Id, aulaAtual.Id);

                var progresso = db.Progressos.FirstOrDefault(p => p.MatriculaId == _matricula.Id);
                if (progresso != null)
                    TxtProgresso.Text = $"{(int)Math.Round(progresso.PercentualConcluido)}%";
            }

            _aulaAtualIndex++;
            AtualizarNavegacao();
            RenderizarAulasSemRecarregar();

            // Abre o player com a próxima aula
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarPlayer(_matricula!, _aulas[_aulaAtualIndex]);
        }

        private void BtnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (_aulaAtualIndex <= 0) return;
            _aulaAtualIndex--;
            AtualizarNavegacao();
            RenderizarAulasSemRecarregar();
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }
    }
}