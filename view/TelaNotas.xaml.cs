using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public void DefinirAluno(Aluno aluno)
        {
            Sidebar.DefinirAluno(aluno.Nome);
            CarregarNotas(aluno.Id);
        }

        // Mantido por compatibilidade
        public void DefinirMatricula(Matricula? matricula)
        {
            if (matricula?.Aluno != null)
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);

            if (matricula == null) return;
            Sidebar.DefinirAluno(matricula.Aluno?.Nome ?? "Aluno");
            CarregarNotas(matricula.AlunoId);
        }

        private void CarregarNotas(int alunoId)
        {
            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == alunoId)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Avaliacoes)
                .ToList();

            if (matriculas.Count == 0 || matriculas.All(m => !m.Avaliacoes.Any()))
            {
                PainelVazio.Visibility = System.Windows.Visibility.Visible;
                ListaNotas.Visibility = System.Windows.Visibility.Collapsed;
                TxtMediaGeral.Text = "—";
                TxtDisciplinas.Text = "0";
                TxtSituacao.Text = "—";
                TxtSituacao.Foreground = new SolidColorBrush(Colors.Gray);
                return;
            }

            PainelVazio.Visibility = System.Windows.Visibility.Collapsed;
            ListaNotas.Visibility = System.Windows.Visibility.Visible;

            var ptBR = new CultureInfo("pt-BR");
            var items = new List<NotaLinhaVM>();

            foreach (var m in matriculas)
            {
                var avs = m.Avaliacoes.OrderBy(a => a.Titulo).ToList();
                double av1 = avs.ElementAtOrDefault(0)?.Nota ?? -1;
                double av2 = avs.ElementAtOrDefault(1)?.Nota ?? -1;
                double av3 = avs.ElementAtOrDefault(2)?.Nota ?? -1;

                var notasValidas = avs.Select(a => a.Nota).ToList();
                bool semNotas = !notasValidas.Any();
                double media = notasValidas.Any() ? notasValidas.Average() : 0;

                bool cursando = semNotas;
                bool aprovado = !semNotas && media >= 7.0;
                bool recuperacao = !semNotas && media >= 5.0 && media < 7.0;

                items.Add(new NotaLinhaVM
                {
                    NomeCurso = m.Curso?.Titulo ?? "Curso",
                    NomeInstrutor = $"Prof. {m.Curso?.Instrutor?.Nome}",
                    NotaAV1 = av1 >= 0 ? av1.ToString("0.0", ptBR) : "—",
                    NotaAV2 = av2 >= 0 ? av2.ToString("0.0", ptBR) : "—",
                    NotaAV3 = av3 >= 0 ? av3.ToString("0.0", ptBR) : "—",
                    Media = media.ToString("0.0", ptBR),
                    CorMedia = new SolidColorBrush(cursando
                                        ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#90CAF9")
                                        : aprovado
                                            ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#A5D6A7")
                                            : recuperacao
                                                ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFCC80")
                                                : (System.Windows.Media.Color)ColorConverter.ConvertFromString("#EF9A9A")),
                    StatusTexto = cursando ? "Cursando" : aprovado ? "Aprovado" : recuperacao ? "Recuperação" : "Reprovado",
                    CorFundoStatus = new SolidColorBrush(cursando
                                        ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#1A2A3A")
                                        : aprovado
                                            ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#1B5E20")
                                            : recuperacao
                                                ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#4E3600")
                                                : (System.Windows.Media.Color)ColorConverter.ConvertFromString("#5E1B1B")),
                    CorTextoStatus = new SolidColorBrush(cursando
                                        ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#90CAF9")
                                        : aprovado
                                            ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#A5D6A7")
                                            : recuperacao
                                                ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFCC80")
                                                : (System.Windows.Media.Color)ColorConverter.ConvertFromString("#EF9A9A")),
                });
            }

            ListaNotas.ItemsSource = items;
            TxtDisciplinas.Text = items.Count.ToString();

            var todasNotas = matriculas.SelectMany(m => m.Avaliacoes).ToList();
            if (todasNotas.Any())
            {
                double mediaGeral = todasNotas.Average(a => a.Nota);
                TxtMediaGeral.Text = mediaGeral.ToString("0.0", ptBR);
                bool geralAprovado = mediaGeral >= 7.0;
                bool geralRecuperacao = mediaGeral >= 5.0 && mediaGeral < 7.0;
                TxtSituacao.Text = geralAprovado ? "Aprovado" : geralRecuperacao ? "Recuperação" : "Reprovado";
                TxtSituacao.Foreground = new SolidColorBrush(geralAprovado
                    ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#A5D6A7")
                    : geralRecuperacao
                        ? (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFCC80")
                        : (System.Windows.Media.Color)ColorConverter.ConvertFromString("#EF9A9A"));
            }
        }
    }

    public class NotaLinhaVM
    {
        public string NomeCurso { get; set; } = "";
        public string NomeInstrutor { get; set; } = "";
        public string NotaAV1 { get; set; } = "—";
        public string NotaAV2 { get; set; } = "—";
        public string NotaAV3 { get; set; } = "—";
        public string Media { get; set; } = "—";
        public SolidColorBrush CorMedia { get; set; } = new();
        public string StatusTexto { get; set; } = "";
        public SolidColorBrush CorFundoStatus { get; set; } = new();
        public SolidColorBrush CorTextoStatus { get; set; } = new();
    }
}