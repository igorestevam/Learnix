using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaNotas : UserControl
    {
        public TelaNotas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula)
        {
            if (matricula?.Aluno != null)
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);

            if (matricula == null) return;

            // Recarrega a matricula com avaliacoes do banco para garantir dados atualizados
            using var ctx = new LearnixDbContext();
            var matriculaCompleta = ctx.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Instrutor)
                .Include(m => m.Avaliacoes)
                .FirstOrDefault(m => m.Id == matricula.Id);

            if (matriculaCompleta == null) return;

            PopularTabela(ctx, matriculaCompleta);
        }

        private void PopularTabela(LearnixDbContext ctx, Matricula matricula)
        {
            // Busca todas as matriculas do aluno para calcular media geral e total
            var todasMatriculas = ctx.Matriculas
                .Include(m => m.Avaliacoes)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Instrutor)
                .Where(m => m.AlunoId == matricula.AlunoId)
                .ToList();

            // Atualiza cards de resumo
            int totalCursos = todasMatriculas.Count;
            double mediaGeral = todasMatriculas
                .Where(m => m.Avaliacoes != null && m.Avaliacoes.Count > 0)
                .Select(m => m.Avaliacoes.Average(a => a.Nota))
                .DefaultIfEmpty(0)
                .Average();

            // Atualiza os TextBlocks dos cards de resumo via FindName
            var txtMediaGeral = FindName("TxtMediaGeral") as TextBlock;
            var txtTotalCursos = FindName("TxtTotalCursos") as TextBlock;

            if (txtMediaGeral != null)
                txtMediaGeral.Text = mediaGeral.ToString("0.0").Replace('.', ',');
            if (txtTotalCursos != null)
                txtTotalCursos.Text = totalCursos.ToString();

            // Limpa e popula a lista de linhas dinamicamente
            var painelLinhas = FindName("PainelLinhas") as StackPanel;
            if (painelLinhas == null) return;

            painelLinhas.Children.Clear();

            foreach (var mat in todasMatriculas)
            {
                var av1 = mat.Avaliacoes?.FirstOrDefault(a => a.Titulo == "AV1");
                var av2 = mat.Avaliacoes?.FirstOrDefault(a => a.Titulo == "AV2");
                var av3 = mat.Avaliacoes?.FirstOrDefault(a => a.Titulo == "AV3");

                double media = mat.NotaFinal;
                bool aprovado = media >= 6.0;
                bool temAvaliacoes = mat.Avaliacoes != null && mat.Avaliacoes.Count > 0;

                string statusTxt = !temAvaliacoes ? "Sem notas" : aprovado ? "Aprovado" : "Recuperacao";
                string statusCor = !temAvaliacoes ? "#555" : aprovado ? "#1B5E20" : "#4E3600";
                string statusFg = !temAvaliacoes ? "#9E8FC0" : aprovado ? "#A5D6A7" : "#FFCC80";

                var linha = CriarLinhaAvaliacao(
                    mat.Curso?.Titulo ?? "Curso",
                    mat.Curso?.Instrutor?.Nome ?? "Instrutor",
                    av1?.Nota, av2?.Nota, av3?.Nota,
                    temAvaliacoes ? media : (double?)null,
                    statusTxt, statusCor, statusFg);

                painelLinhas.Children.Add(linha);
            }
        }

        private Border CriarLinhaAvaliacao(
            string nomeCurso, string nomeInstrutor,
            double? av1, double? av2, double? av3, double? media,
            string statusTxt, string statusCor, string statusFg)
        {
            string FormatarNota(double? nota)
                => nota.HasValue ? nota.Value.ToString("0.0").Replace('.', ',') : "—";

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Coluna 0: nome do curso + instrutor
            var stackNome = new StackPanel();
            stackNome.Children.Add(new TextBlock
            {
                Text = nomeCurso,
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Segoe UI"),
                TextWrapping = TextWrapping.Wrap
            });
            stackNome.Children.Add(new TextBlock
            {
                Text = "Prof. " + nomeInstrutor,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                FontSize = 11,
                FontFamily = new FontFamily("Segoe UI"),
                Margin = new Thickness(0, 2, 0, 0)
            });
            Grid.SetColumn(stackNome, 0);
            grid.Children.Add(stackNome);

            // Colunas 1-3: AV1, AV2, AV3
            string[] notas = { FormatarNota(av1), FormatarNota(av2), FormatarNota(av3) };
            for (int i = 0; i < 3; i++)
            {
                var tb = new TextBlock
                {
                    Text = notas[i],
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8CCF0")),
                    FontSize = 13,
                    FontFamily = new FontFamily("Segoe UI"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(tb, i + 1);
                grid.Children.Add(tb);
            }

            // Coluna 4: Média
            var tbMedia = new TextBlock
            {
                Text = FormatarNota(media),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                    media.HasValue && media.Value >= 6.0 ? "#A5D6A7" : "#FFCC80")),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Segoe UI"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(tbMedia, 4);
            grid.Children.Add(tbMedia);

            // Coluna 5: Status
            var borderStatus = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(statusCor)),
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(10, 4, 10, 4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            borderStatus.Child = new TextBlock
            {
                Text = statusTxt,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(statusFg)),
                FontSize = 11,
                FontFamily = new FontFamily("Segoe UI")
            };
            Grid.SetColumn(borderStatus, 5);
            grid.Children.Add(borderStatus);

            return new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2040")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(18, 12, 18, 12),
                Margin = new Thickness(0, 0, 0, 8),
                Child = grid
            };
        }
    }
}
