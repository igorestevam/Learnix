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
    public partial class TelaNotas : UserControl
    {
        private readonly MatriculaController _matriculaController = new();

        public TelaNotas()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            Sidebar.DefinirAluno(aluno.Nome);
            CarregarNotas(aluno.Id);
        }

        public void DefinirMatricula(Matricula? matricula)
        {
            if (matricula?.Aluno != null)
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);

            if (matricula == null) return;
            CarregarNotas(matricula.AlunoId);
        }

        private void CarregarNotas(int alunoId)
        {
            var matriculas = _matriculaController.ListarAtivas(alunoId);

            if (matriculas.Count == 0)
            {
                PainelVazio.Visibility = Visibility.Visible;
                ListaNotas.Visibility = Visibility.Collapsed;
                TxtMediaGeral.Text = "—";
                TxtDisciplinas.Text = "0";
                TxtSituacao.Text = "—";
                TxtSituacao.Foreground = new SolidColorBrush(Colors.Gray);
                return;
            }

            PainelVazio.Visibility = Visibility.Collapsed;
            ListaNotas.Visibility = Visibility.Visible;

            var ptBR = new CultureInfo("pt-BR");
            var items = new List<NotaLinhaVM>();
            var notasFinais = new List<double>();

            foreach (var m in matriculas)
            {
                var avaliacoes = m.Avaliacoes?.OrderBy(a => a.Titulo).ToList()
                                 ?? new List<Avaliacao>();
                bool avaliado = avaliacoes.Any();

                string strAv1 = "—", strAv2 = "—", strAv3 = "—", strMedia = "—";

                if (avaliado)
                {
                    double nota1 = avaliacoes.ElementAtOrDefault(0)?.Nota ?? 0;
                    double nota2 = avaliacoes.ElementAtOrDefault(1)?.Nota ?? 0;
                    double nota3 = avaliacoes.ElementAtOrDefault(2)?.Nota ?? 0;

                    strAv1 = nota1.ToString("0.0", ptBR);
                    strAv2 = nota2.ToString("0.0", ptBR);
                    strAv3 = nota3.ToString("0.0", ptBR);

                    double media = avaliacoes.Average(a => a.Nota);
                    strMedia = media.ToString("0.0", ptBR);
                    notasFinais.Add(media);
                }

                string statusTexto = "Cursando";
                string corTextoHex = "#90CAF9";
                string corFundoHex = "#1A2A3A";

                if (m.Status == StatusMatricula.AguardandoCorrecao)
                {
                    statusTexto = "Em Correção";
                    corTextoHex = "#FFCA28";
                    corFundoHex = "#4E3600";
                }
                else if (m.Status == StatusMatricula.Concluida)
                {
                    statusTexto = "Aprovado";
                    corTextoHex = "#A5D6A7";
                    corFundoHex = "#1B5E20";
                }
                else if (m.Status == StatusMatricula.Reprovada)
                {
                    statusTexto = "Reprovado";
                    corTextoHex = "#EF9A9A";
                    corFundoHex = "#5E1B1B";
                }

                items.Add(new NotaLinhaVM
                {
                    NomeCurso = m.Curso?.Titulo ?? "Curso",
                    NomeInstrutor = $"Prof. {m.Curso?.Instrutor?.Nome}",
                    NotaAV1 = strAv1,
                    NotaAV2 = strAv2,
                    NotaAV3 = strAv3,
                    Media = strMedia,
                    CorMedia = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        avaliado ? corTextoHex : "#90CAF9")),
                    StatusTexto = statusTexto,
                    CorFundoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundoHex)),
                    CorTextoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTextoHex)),
                });
            }

            ListaNotas.ItemsSource = items;
            TxtDisciplinas.Text = items.Count.ToString();

            if (notasFinais.Any())
            {
                double mediaGeral = notasFinais.Average();
                TxtMediaGeral.Text = mediaGeral.ToString("0.0", ptBR);

                bool aprovado = mediaGeral >= 7.0;
                TxtSituacao.Text = aprovado ? "Aprovado" : "Reprovado";
                TxtSituacao.Foreground = new SolidColorBrush(aprovado
                    ? (Color)ColorConverter.ConvertFromString("#A5D6A7")
                    : (Color)ColorConverter.ConvertFromString("#EF9A9A"));
            }
            else
            {
                TxtMediaGeral.Text = "—";
                TxtSituacao.Text = "Cursando";
                TxtSituacao.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#90CAF9"));
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
