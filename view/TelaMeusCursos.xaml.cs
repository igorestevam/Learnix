using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private readonly MatriculaController _matriculaController = new();
        private readonly AvaliacaoController _avaliacaoController = new();

        private Aluno? _aluno;
        private int _matriculaAvaliacaoAtualId;
        private int _atividade1Id;
        private int _atividade2Id;
        private int _atividade3Id;

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            Sidebar?.DefinirAluno(aluno.Nome);
            CarregarCursos();
        }

        private void CarregarCursos()
        {
            if (_aluno == null) return;

            var matriculas = _matriculaController.ListarAtivas(_aluno.Id);

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
                double pct = m.Progresso?.PercentualConcluido ?? 0;
                bool concluido = pct >= 100 || m.Status == StatusMatricula.Concluida;
                int totalAulas = m.Curso?.Modulos?.Sum(mod => mod.Aulas?.Count ?? 0) ?? 0;
                string categoria = m.Curso?.Categoria?.Nome ?? "Geral";

                string corFundo = categoria switch { "Humanas" => "#1A3A2A", "Tecnologia" => "#1A2A3A", _ => "#3A2860" };
                string corTexto = categoria switch { "Humanas" => "#A5D6A7", "Tecnologia" => "#90CAF9", _ => "#D8CCF0" };
                double largura = Math.Min(pct / 100.0 * 300, 300);

                var vm = new CursoCardVM
                {
                    MatriculaId = m.Id,
                    CursoId = m.CursoId,
                    TituloCurso = m.Curso?.Titulo ?? "Curso",
                    NomeInstrutor = m.Curso?.Instrutor != null ? $"Prof. {m.Curso.Instrutor.Nome}" : "",
                    NomeCategoria = categoria,
                    CorCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    DataFormatada = $"📅 {m.DataMatricula:MMM yyyy}",
                    CargaHoraria = $"🕐 {m.Curso?.CargaHoraria ?? 0}h",
                    NumAulas = $"✏️ {totalAulas} aulas",
                    PercentualTexto = concluido ? "100% ✔" : $"{pct:0}%",
                    LarguraBarra = largura,
                    CorBarra = new SolidColorBrush((Color)ColorConverter.ConvertFromString(concluido ? "#A5D6A7" : "#7E6BAC")),
                    CorFundoBarra = new SolidColorBrush((Color)ColorConverter.ConvertFromString(concluido ? "#1B5E20" : "#3A2860")),
                    CorTextoProgresso = new SolidColorBrush((Color)ColorConverter.ConvertFromString(concluido ? "#A5D6A7" : "#D8CCF0")),
                };

                if (m.Status == StatusMatricula.Reprovada)
                {
                    vm.BotaoSairVisivel = Visibility.Visible;
                    vm.TextoStatusVisivel = Visibility.Visible;
                    vm.TextoStatusMsg = "❌ Você não atingiu a média. Abandone e tente novamente.";
                    vm.CorTextoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF5350"));
                }
                else if (m.Status == StatusMatricula.Concluida)
                {
                    vm.BotaoAcessarVisivel = Visibility.Visible;
                    vm.BotaoCertificadoVisivel = Visibility.Visible;
                }
                else if (m.Status == StatusMatricula.AguardandoCorrecao)
                {
                    vm.BotaoAcessarVisivel = Visibility.Visible;
                    vm.TextoStatusVisivel = Visibility.Visible;
                    vm.TextoStatusMsg = "⏳ Aguardando revisão do instrutor...";
                    vm.CorTextoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCA28"));
                }
                else if (m.Status == StatusMatricula.AguardandoContinuar)
                {
                    vm.BotaoAcessarVisivel = Visibility.Visible;
                    vm.BotaoAvaliacaoVisivel = Visibility.Visible;
                    vm.TextoStatusVisivel = Visibility.Visible;
                    vm.TextoStatusMsg = "✅ Aulas concluídas! Responda às atividades avaliativas.";
                    vm.CorTextoStatus = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5D6A7"));
                }
                else
                {
                    vm.BotaoAcessarVisivel = Visibility.Visible;

                    if (pct >= 100)
                        vm.BotaoAvaliacaoVisivel = Visibility.Visible;
                }

                return vm;
            }).ToList();

            ListaCursos.ItemsSource = items;
        }

        private void BtnAcessar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            var matricula = _matriculaController.BuscarCompleta(matriculaId);
            if (matricula == null) return;

            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarAulas(matricula);
        }

        private void BtnCertificado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            int? certId = _matriculaController.BuscarCertificadoId(matriculaId);
            var main = Application.Current.MainWindow as MainWindow;

            if (main != null && certId.HasValue && _aluno != null)
                main.MostrarCertificadosPorId(_aluno, certId.Value);
        }

        private void BtnSairCurso_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            var r = MessageBox.Show(
                "Deseja realmente sair deste curso?\nSeu progresso será perdido e a matrícula será cancelada.",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;

            _matriculaController.Cancelar(matriculaId);

            MessageBox.Show("Sua matrícula foi cancelada com sucesso. Você poderá reingressar no curso quando quiser.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregarCursos();
        }

        public void AbrirAvaliacaoFinal(int matriculaId)
        {
            var matricula = _matriculaController.BuscarComAtividades(matriculaId);

            if (matricula == null || matricula.Curso.Atividades.Count < 3)
            {
                MessageBox.Show("Este curso ainda não possui as 3 avaliações configuradas pelo instrutor.",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var atividades = matricula.Curso.Atividades.ToList();

            _matriculaAvaliacaoAtualId = matriculaId;
            _atividade1Id = atividades[0].Id;
            _atividade2Id = atividades[1].Id;
            _atividade3Id = atividades[2].Id;

            TxtPerguntaAluno1.Text = $"1. {atividades[0].Pergunta}";
            TxtPerguntaAluno2.Text = $"2. {atividades[1].Pergunta}";
            TxtPerguntaAluno3.Text = $"3. {atividades[2].Pergunta}";

            TxtRespostaAluno1.Clear();
            TxtRespostaAluno2.Clear();
            TxtRespostaAluno3.Clear();

            PainelAvaliacaoAluno.Visibility = Visibility.Visible;
        }

        private void BtnEnviarAvaliacao_Click(object sender, RoutedEventArgs e)
        {
            string resp1 = TxtRespostaAluno1.Text.Trim();
            string resp2 = TxtRespostaAluno2.Text.Trim();
            string resp3 = TxtRespostaAluno3.Text.Trim();

            if (string.IsNullOrEmpty(resp1) || string.IsNullOrEmpty(resp2) || string.IsNullOrEmpty(resp3))
            {
                MessageBox.Show("Você precisa responder todas as 3 perguntas para concluir a avaliação.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool enviado = _avaliacaoController.EnviarRespostas(
                _matriculaAvaliacaoAtualId,
                _atividade1Id, resp1,
                _atividade2Id, resp2,
                _atividade3Id, resp3);

            if (!enviado)
            {
                MessageBox.Show("Você já enviou a avaliação para este curso!",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                PainelAvaliacaoAluno.Visibility = Visibility.Collapsed;
                return;
            }

            MessageBox.Show("Parabéns! Suas respostas foram enviadas e o instrutor já pode avaliá-las.",
                "Avaliação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            PainelAvaliacaoAluno.Visibility = Visibility.Collapsed;
        }

        private void BtnCancelarAvaliacao_Click(object sender, RoutedEventArgs e)
        {
            PainelAvaliacaoAluno.Visibility = Visibility.Collapsed;
        }

        private void BtnAbrirAvaliacao_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int matriculaId)
                AbrirAvaliacaoFinal(matriculaId);
        }

        private void BtnContinuarParaAvaliacao_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            _matriculaController.AtualizarStatus(matriculaId, StatusMatricula.AguardandoContinuar);
            CarregarCursos();
            MessageBox.Show("Você concluiu as aulas! Agora você pode responder às atividades avaliativas.",
                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class CursoCardVM
    {
        public int MatriculaId { get; set; }
        public int CursoId { get; set; }
        public string TituloCurso { get; set; } = "";
        public string NomeInstrutor { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public SolidColorBrush CorCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public string DataFormatada { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public string NumAulas { get; set; } = "";
        public string PercentualTexto { get; set; } = "";
        public string TextoStatusMsg { get; set; } = "";
        public double LarguraBarra { get; set; }
        public SolidColorBrush CorBarra { get; set; } = new();
        public SolidColorBrush CorFundoBarra { get; set; } = new();
        public SolidColorBrush CorTextoProgresso { get; set; } = new();
        public SolidColorBrush CorTextoStatus { get; set; } = new();
        public Visibility BotaoAcessarVisivel { get; set; } = Visibility.Collapsed;
        public Visibility BotaoAvaliacaoVisivel { get; set; } = Visibility.Collapsed;
        public Visibility BotaoCertificadoVisivel { get; set; } = Visibility.Collapsed;
        public Visibility BotaoSairVisivel { get; set; } = Visibility.Collapsed;
        public Visibility TextoStatusVisivel { get; set; } = Visibility.Collapsed;
    }
}
