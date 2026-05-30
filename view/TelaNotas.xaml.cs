using System;
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

        public void DefinirMatricula(Matricula? matricula)
        {
            if (matricula?.Aluno != null)
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);

            if (matricula == null) return;
            CarregarNotas(matricula.AlunoId);
        }

        private void CarregarNotas(int alunoId)
        {
            using var db = new LearnixDbContext();

            // 1. Busca matrículas que não estejam Canceladas
            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == alunoId && m.Status != StatusMatricula.Cancelada)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .ToList();

            if (matriculas.Count == 0)
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

            // 2. Busca TODAS as respostas (provas) dessas matrículas para extrair as notas
            var matriculaIds = matriculas.Select(m => m.Id).ToList();
            var todasRespostas = db.RespostasAtividades
                .Where(r => matriculaIds.Contains(r.MatriculaId))
                .ToList();

            var ptBR = new CultureInfo("pt-BR");
            var items = new List<NotaLinhaVM>();
            var notasFinais = new List<decimal>();

            foreach (var m in matriculas)
            {
                // Ordena as respostas pelo ID (garantindo ordem Q1, Q2, Q3)
                var respostas = todasRespostas.Where(r => r.MatriculaId == m.Id).OrderBy(r => r.Id).ToList();

                // Só exibimos notas se o professor já tiver concluído ou reprovado o aluno
                bool avaliado = m.Status == StatusMatricula.Concluida || m.Status == StatusMatricula.Reprovada;

                string strAv1 = "—", strAv2 = "—", strAv3 = "—", strMedia = "—";

                if (avaliado)
                {
                    decimal nota1 = respostas.ElementAtOrDefault(0)?.Nota ?? 0m;
                    decimal nota2 = respostas.ElementAtOrDefault(1)?.Nota ?? 0m;
                    decimal nota3 = respostas.ElementAtOrDefault(2)?.Nota ?? 0m;

                    strAv1 = nota1.ToString("0.0", ptBR);
                    strAv2 = nota2.ToString("0.0", ptBR);
                    strAv3 = nota3.ToString("0.0", ptBR);

                    decimal media = (nota1 + nota2 + nota3) / 3m;
                    strMedia = media.ToString("0.0", ptBR);
                    notasFinais.Add(media);
                }

                // 3. Define as Cores e Textos baseados no Status Inteligente
                string statusTexto = "Cursando";
                string corTextoHex = "#90CAF9";  // Azul
                string corFundoHex = "#1A2A3A";

                if (m.Status == StatusMatricula.AguardandoCorrecao)
                {
                    statusTexto = "Em Correção";
                    corTextoHex = "#FFCA28"; // Amarelo
                    corFundoHex = "#4E3600";
                }
                else if (m.Status == StatusMatricula.Concluida)
                {
                    statusTexto = "Aprovado";
                    corTextoHex = "#A5D6A7"; // Verde
                    corFundoHex = "#1B5E20";
                }
                else if (m.Status == StatusMatricula.Reprovada)
                {
                    statusTexto = "Reprovado";
                    corTextoHex = "#EF9A9A"; // Vermelho
                    corFundoHex = "#5E1B1B";
                }

                var corMediaBrush = avaliado ? corTextoHex : "#90CAF9";

                items.Add(new NotaLinhaVM
                {
                    NomeCurso = m.Curso?.Titulo ?? "Curso",
                    NomeInstrutor = $"Prof. {m.Curso?.Instrutor?.Nome}",
                    NotaAV1 = strAv1,
                    NotaAV2 = strAv2,
                    NotaAV3 = strAv3,
                    Media = strMedia,
                    CorMedia = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corMediaBrush)),
                    StatusTexto = statusTexto,
                    CorFundoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundoHex)),
                    CorTextoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTextoHex))
                });
            }

            ListaNotas.ItemsSource = items;
            TxtDisciplinas.Text = items.Count.ToString();

            // 4. Calcula a Média Geral global (somente de cursos que já receberam notas)
            if (notasFinais.Any())
            {
                decimal mediaGeral = notasFinais.Average();
                TxtMediaGeral.Text = mediaGeral.ToString("0.0", ptBR);

                bool geralAprovado = mediaGeral >= 7.0m;
                TxtSituacao.Text = geralAprovado ? "Aprovado" : "Reprovado";
                TxtSituacao.Foreground = new SolidColorBrush(geralAprovado
                    ? (Color)ColorConverter.ConvertFromString("#A5D6A7")
                    : (Color)ColorConverter.ConvertFromString("#EF9A9A"));
            }
            else
            {
                // Se nenhum curso foi avaliado ainda
                TxtMediaGeral.Text = "—";
                TxtSituacao.Text = "Cursando";
                TxtSituacao.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90CAF9"));
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