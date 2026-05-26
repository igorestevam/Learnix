using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private string _categoriaAtiva = "Todos";
        private string _busca = "";
        private const string PlaceholderBusca = "Buscar curso...";
        private Aluno? _aluno;
        private List<MenuCursoVM> _todosCursos = new();

        public TelaMenu()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            CarregarCursos();
        }

        private void CarregarCursos()
        {
            using var db = new LearnixDbContext();

            var matriculasDoAluno = _aluno != null
                ? db.Matriculas.Where(m => m.AlunoId == _aluno.Id)
                               .Select(m => m.CursoId).ToHashSet()
                : new HashSet<int>();

            var cursos = db.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Include(c => c.MatriculasAtivas)
                .ToList();

            _todosCursos = cursos.Select(c =>
            {
                bool jaMatriculado = matriculasDoAluno.Contains(c.Id);
                string categoria = c.Categoria?.Nome ?? "Geral";

                string corFundo = categoria switch
                {
                    "Humanas" => "#1A3A2A",
                    "Tecnologia" => "#1A2A3A",
                    _ => "#3A2860"
                };
                string corTexto = categoria switch
                {
                    "Humanas" => "#A5D6A7",
                    "Tecnologia" => "#90CAF9",
                    _ => "#D8CCF0"
                };

                return new MenuCursoVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    NomeCategoria = categoria,
                    CargaHoraria = $"{c.CargaHoraria}h",
                    NomeInstrutor = $"Prof. {c.Instrutor?.Nome}",
                    Descricao = c.Descricao,
                    NumAlunos = $"👥 {c.MatriculasAtivas?.Count ?? 0} alunos",
                    CorFundoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    TextoBotao = jaMatriculado ? "✔ Matriculado" : "Matricular-se",
                    CorBotao = new SolidColorBrush(jaMatriculado
                                           ? (Color)ColorConverter.ConvertFromString("#1B5E20")
                                           : (Color)ColorConverter.ConvertFromString("#4E3A7A")),
                    BotaoAtivo = !jaMatriculado,
                };
            }).ToList();

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var filtrados = _todosCursos.Where(c =>
            {
                bool passaCategoria = _categoriaAtiva == "Todos" || c.NomeCategoria == _categoriaAtiva;
                bool passaBusca = string.IsNullOrEmpty(_busca) || c.Titulo.ToLower().Contains(_busca);
                return passaCategoria && passaBusca;
            }).ToList();

            if (filtrados.Count == 0)
            {
                PainelVazio.Visibility = Visibility.Visible;
                ListaCursos.Visibility = Visibility.Collapsed;
            }
            else
            {
                PainelVazio.Visibility = Visibility.Collapsed;
                ListaCursos.Visibility = Visibility.Visible;
                ListaCursos.ItemsSource = filtrados;
            }
        }

        // ── Busca ────────────────────────────────────────────────────────────

        private void TxtBusca_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtBusca.Text == PlaceholderBusca)
            {
                TxtBusca.Text = "";
                TxtBusca.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void TxtBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBusca.Text))
            {
                TxtBusca.Text = PlaceholderBusca;
                TxtBusca.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#9E8FC0"));
            }
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtBusca.Text == PlaceholderBusca) return;
            _busca = TxtBusca.Text.ToLower().Trim();
            AplicarFiltros();
        }

        // ── Filtros ──────────────────────────────────────────────────────────

        private void FiltroTodos_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Todos");
        private void FiltroExatas_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Exatas");
        private void FiltroHumanas_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Humanas");
        private void FiltroTecnologia_Click(object sender, MouseButtonEventArgs e)
            => AtivarCategoria("Tecnologia");

        private void AtivarCategoria(string categoria)
        {
            _categoriaAtiva = categoria;
            var inativa = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2040"));
            var ativa = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A"));
            BtnTodos.Background = inativa;
            BtnExatas.Background = inativa;
            BtnHumanas.Background = inativa;
            BtnTecnologia.Background = inativa;
            switch (categoria)
            {
                case "Todos": BtnTodos.Background = ativa; break;
                case "Exatas": BtnExatas.Background = ativa; break;
                case "Humanas": BtnHumanas.Background = ativa; break;
                case "Tecnologia": BtnTecnologia.Background = ativa; break;
            }
            AplicarFiltros();
        }

        // ── Matrícula ────────────────────────────────────────────────────────

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (_aluno == null)
            {
                MessageBox.Show("Usuário não identificado.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (sender is not Button btn || btn.Tag is not int cursoId) return;

            using var db = new LearnixDbContext();

            var curso = db.Cursos.FirstOrDefault(c => c.Id == cursoId);
            if (curso == null) return;

            bool jaMatriculado = db.Matriculas.Any(m => m.AlunoId == _aluno.Id && m.CursoId == cursoId);
            if (jaMatriculado)
            {
                MessageBox.Show($"Você já está matriculado em \"{curso.Titulo}\".",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var matricula = new Matricula
            {
                AlunoId = _aluno.Id,
                CursoId = cursoId,
                DataMatricula = System.DateTime.Now,
                Status = StatusMatricula.Ativa,
                Progresso = new Progresso { PercentualConcluido = 0, AulasConcluidas = 0 },
            };

            db.Matriculas.Add(matricula);
            db.SaveChanges();

            // Atualiza o card visualmente
            btn.Content = "✔ Matriculado";
            btn.IsEnabled = false;

            MessageBox.Show($"Matrícula em \"{curso.Titulo}\" realizada com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class MenuCursoVM
    {
        public int CursoId { get; set; }
        public string Titulo { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public string NomeInstrutor { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string NumAlunos { get; set; } = "";
        public SolidColorBrush CorFundoCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public string TextoBotao { get; set; } = "Matricular-se";
        public SolidColorBrush CorBotao { get; set; } = new();
        public bool BotaoAtivo { get; set; } = true;
    }
}