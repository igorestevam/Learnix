using System;
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
    public partial class TelaHome : UserControl
    {
        public TelaHome()
        {
            InitializeComponent();
            TxtData.Text = DateTime.Now.ToString(
                "dddd, dd 'de' MMMM 'de' yyyy",
                new CultureInfo("pt-BR"));
        }

        public void DefinirAluno(string nome)
        {
            TxtNomeAluno.Text = nome;
            Sidebar.DefinirAluno(nome);
        }

        /// <summary>
        /// Sobrecarga que recebe o Aluno completo e popula os cards com dados reais do banco.
        /// </summary>
        public void DefinirAluno(Aluno aluno)
        {
            TxtNomeAluno.Text = aluno.Nome;
            Sidebar.DefinirAluno(aluno.Nome);

            using var ctx = new LearnixDbContext();
            var alunoCompleto = ctx.Alunos
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Progresso)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Avaliacoes)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Instrutor)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Categoria)
                .FirstOrDefault(a => a.Id == aluno.Id);

            if (alunoCompleto == null) return;

            var matriculas = alunoCompleto.HistoricoMatriculas ?? new System.Collections.Generic.List<Matricula>();

            // Card: Cursos Ativos
            int cursosAtivos = matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            if (FindName("TxtCursosAtivos") is TextBlock txtCursos)
                txtCursos.Text = cursosAtivos.ToString();

            // Card: Média Geral
            var todasAvaliacoes = matriculas.SelectMany(m => m.Avaliacoes ?? new System.Collections.Generic.List<Avaliacao>()).ToList();
            double mediaGeral = todasAvaliacoes.Count > 0 ? todasAvaliacoes.Average(a => a.Nota) : 0.0;
            if (FindName("TxtMediaGeralHome") is TextBlock txtMedia)
                txtMedia.Text = mediaGeral.ToString("0.0").Replace('.', ',');

            // Card: Certificados
            int totalCerts = matriculas.Count(m => m.Status == StatusMatricula.Concluida);
            if (FindName("TxtTotalCertificados") is TextBlock txtCerts)
                txtCerts.Text = totalCerts.ToString();

            // Seção: Continuar estudando — popula o painel dinâmico
            if (FindName("PainelCursos") is StackPanel painel)
            {
                painel.Children.Clear();
                var ativas = matriculas.Where(m => m.Status == StatusMatricula.Ativa).Take(3).ToList();

                if (ativas.Count == 0)
                {
                    painel.Children.Add(new TextBlock
                    {
                        Text = "Nenhum curso ativo. Acesse o Menu para se matricular!",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                        FontSize = 13,
                        FontFamily = new FontFamily("Segoe UI"),
                        Margin = new Thickness(0, 8, 0, 0)
                    });
                }
                else
                {
                    foreach (var mat in ativas)
                        painel.Children.Add(CriarCardCurso(mat));
                }
            }
        }

        private Border CriarCardCurso(Matricula mat)
        {
            double percentual = mat.Progresso?.PercentualConcluido ?? 0.0;
            string instrutor = mat.Curso?.Instrutor?.Nome ?? "Instrutor";
            string categoria = mat.Curso?.Categoria?.Nome ?? "";

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Info do curso
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = mat.Curso?.Titulo ?? "Curso",
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Segoe UI")
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"Prof. {instrutor}  •  {categoria}",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                FontSize = 12,
                FontFamily = new FontFamily("Segoe UI"),
                Margin = new Thickness(0, 4, 0, 10)
            });

            // Barra de progresso
            double larguraBarra = Math.Max(4, (percentual / 100.0) * 280);
            var gridBarra = new Grid { Height = 6 };
            gridBarra.Children.Add(new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2860")),
                CornerRadius = new CornerRadius(3)
            });
            gridBarra.Children.Add(new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7E6BAC")),
                CornerRadius = new CornerRadius(3),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = larguraBarra
            });
            stack.Children.Add(gridBarra);
            stack.Children.Add(new TextBlock
            {
                Text = $"{(int)percentual}% concluído",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                FontSize = 11,
                FontFamily = new FontFamily("Segoe UI"),
                Margin = new Thickness(0, 4, 0, 0)
            });

            Grid.SetColumn(stack, 0);
            grid.Children.Add(stack);

            // Botão Continuar
            var btn = new Button
            {
                Content = "Continuar →",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(16, 8, 16, 8),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = mat
            };
            btn.Click += (s, e) =>
            {
                if (s is Button b && b.Tag is Matricula m)
                {
                    var main = Application.Current.MainWindow as MainWindow;
                    main?.MostrarAulas(m);
                }
            };
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            return new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2040")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(18, 14, 18, 14),
                Margin = new Thickness(0, 0, 0, 10),
                Child = grid
            };
        }
    }
}
