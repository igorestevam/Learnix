using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private Aluno? _aluno;

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            Sidebar.DefinirAluno(aluno.Nome);
            CarregarCursos();
        }

        private void CarregarCursos()
        {
            if (_aluno == null) return;

            using var db = new LearnixDbContext();

            var matriculas = db.Matriculas
                .Where(m => m.AlunoId == _aluno.Id)
                .Include(m => m.Curso).ThenInclude(c => c.Categoria)
                .Include(m => m.Curso).ThenInclude(c => c.Instrutor)
                .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
                .Include(m => m.Progresso)
                .ToList();

            if (matriculas.Count == 0)
            {
                PainelVazio.Visibility = Visibility.Visible;
                ListaCursos.Visibility = Visibility.Collapsed;
                return;
            }

            PainelVazio.Visibility = Visibility.Collapsed;
            ListaCursos.Visibility = Visibility.Visible;

            var items = matriculas.Select(m =>
            {
                double pct      = m.Progresso?.PercentualConcluido ?? 0;
                bool concluido  = pct >= 100 || m.Status == StatusMatricula.Concluida;
                int totalAulas  = m.Curso?.Modulos?.Sum(mod => mod.Aulas?.Count ?? 0) ?? 0;
                string categoria = m.Curso?.Categoria?.Nome ?? "Geral";

                string corFundo = categoria switch
                {
                    "Humanas"    => "#1A3A2A",
                    "Tecnologia" => "#1A2A3A",
                    _            => "#3A2860"
                };
                string corTexto = categoria switch
                {
                    "Humanas"    => "#A5D6A7",
                    "Tecnologia" => "#90CAF9",
                    _            => "#D8CCF0"
                };

                double largura = Math.Min(pct / 100.0 * 300, 300);

                return new CursoCardVM
                {
                    MatriculaId         = m.Id,
                    TituloCurso         = m.Curso?.Titulo ?? "Curso",
                    NomeInstrutor       = m.Curso?.Instrutor != null ? $"Prof. {m.Curso.Instrutor.Nome}" : "",
                    NomeCategoria       = categoria,
                    CorCategoria        = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria   = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    DataFormatada       = $"📅 {m.DataMatricula:MMM yyyy}",
                    CargaHoraria        = $"🕐 {m.Curso?.CargaHoraria ?? 0}h",
                    NumAulas            = $"✏️ {totalAulas} aulas",
                    PercentualTexto     = concluido ? "100% ✔" : $"{pct:0}%",
                    LarguraBarra        = largura,
                    CorBarra            = new SolidColorBrush(concluido
                                            ? (Color)ColorConverter.ConvertFromString("#A5D6A7")
                                            : (Color)ColorConverter.ConvertFromString("#7E6BAC")),
                    CorFundoBarra       = new SolidColorBrush(concluido
                                            ? (Color)ColorConverter.ConvertFromString("#1B5E20")
                                            : (Color)ColorConverter.ConvertFromString("#3A2860")),
                    CorTextoProgresso   = new SolidColorBrush(concluido
                                            ? (Color)ColorConverter.ConvertFromString("#A5D6A7")
                                            : (Color)ColorConverter.ConvertFromString("#D8CCF0")),
                    TextoBotao          = concluido ? "🏆 Concluído" : "Acessar →",
                    CorBotao            = new SolidColorBrush(concluido
                                            ? (Color)ColorConverter.ConvertFromString("#2E7D32")
                                            : (Color)ColorConverter.ConvertFromString("#4E3A7A")),
                };
            }).ToList();

            ListaCursos.ItemsSource = items;
        }

        private void BtnAcao_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            using var db = new LearnixDbContext();
            var matricula = db.Matriculas
                .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
                .Include(m => m.Progresso)
                .Include(m => m.Certificado)
                .FirstOrDefault(m => m.Id == matriculaId);

            if (matricula == null) return;

            double pct       = matricula.Progresso?.PercentualConcluido ?? 0;
            bool concluido   = pct >= 100 || matricula.Status == StatusMatricula.Concluida;
            var main         = Application.Current.MainWindow as MainWindow;

            if (concluido)
                main?.MostrarCertificados(_aluno?.Nome ?? "");
            else
                main?.MostrarAulas(matricula);
        }
    }

    public class CursoCardVM
    {
        public int             MatriculaId       { get; set; }
        public string          TituloCurso       { get; set; } = "";
        public string          NomeInstrutor     { get; set; } = "";
        public string          NomeCategoria     { get; set; } = "";
        public SolidColorBrush CorCategoria      { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public string          DataFormatada     { get; set; } = "";
        public string          CargaHoraria      { get; set; } = "";
        public string          NumAulas          { get; set; } = "";
        public string          PercentualTexto   { get; set; } = "";
        public double          LarguraBarra      { get; set; }
        public SolidColorBrush CorBarra          { get; set; } = new();
        public SolidColorBrush CorFundoBarra     { get; set; } = new();
        public SolidColorBrush CorTextoProgresso { get; set; } = new();
        public string          TextoBotao        { get; set; } = "";
        public SolidColorBrush CorBotao          { get; set; } = new();
    }
}
