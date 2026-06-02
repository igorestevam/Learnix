using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private readonly CursoController _cursoController = new();
        private readonly MatriculaController _matriculaController = new();

        private Aluno? _aluno;
        private List<CursoMenuVM> _todosCursos = new();

        public TelaMenu()
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

            var cursosNoBanco = _cursoController.ListarComMatriculas();
            var historicoAluno = _matriculaController.ListarHistorico(_aluno.Id);

            _todosCursos = cursosNoBanco.Select(c =>
            {
                var matricula = historicoAluno.FirstOrDefault(m => m.CursoId == c.Id);

                bool podeMatricular = matricula == null ||
                                      matricula.Status == StatusMatricula.Cancelada ||
                                      matricula.Status == StatusMatricula.Reprovada;

                string categoria = c.Categoria?.Nome ?? "Geral";
                string corFundo = categoria switch { "Humanas" => "#1A3A2A", "Tecnologia" => "#1A2A3A", _ => "#3A2860" };
                string corTexto = categoria switch { "Humanas" => "#A5D6A7", "Tecnologia" => "#90CAF9", _ => "#D8CCF0" };

                return new CursoMenuVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    NomeInstrutor = c.Instrutor != null ? $"Prof. {c.Instrutor.Nome}" : "Sem instrutor vinculado",
                    NomeCategoria = categoria,
                    DescricaoCategoria = string.IsNullOrWhiteSpace(c.Categoria?.Descricao)
                        ? null : c.Categoria.Descricao,
                    CargaHoraria = $"🕐 {c.CargaHoraria}h",
                    NumAlunos = $"👥 {c.MatriculasAtivas?.Count ?? 0} aluno(s)",
                    Preco = c.Preco > 0
                        ? $"💰 R$ {c.Preco:F2}"
                        : "💰 Gratuito",
                    CorFundoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    BotaoAtivo = podeMatricular,
                    TextoBotao = podeMatricular ? "Matricular-se" : "Já Matriculado",
                    CorBotao = podeMatricular
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"))
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555")),
                };
            }).ToList();

            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var filtrados = _todosCursos
                .Where(c => TxtBusca.Text == "Buscar curso..." ||
                            string.IsNullOrWhiteSpace(TxtBusca.Text) ||
                            c.Titulo.Contains(TxtBusca.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            PainelVazio.Visibility = filtrados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaCursos.Visibility = filtrados.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (filtrados.Count > 0)
                ListaCursos.ItemsSource = filtrados;
        }

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int cursoId) return;
            if (_aluno == null) return;

            var result = MessageBox.Show("Deseja confirmar a sua matrícula neste curso?",
                "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            bool reativada = _matriculaController.Matricular(_aluno.Id, cursoId);

            if (reativada)
                MessageBox.Show("Sua matrícula foi reativada! Você iniciará o curso do zero.",
                    "Bons Estudos", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Matrícula realizada com sucesso! Acesse 'Meus Cursos' para começar.",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregarCursos();
        }

        private void TxtBusca_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtBusca.Text == "Buscar curso...")
            {
                TxtBusca.Text = "";
                TxtBusca.Foreground = Brushes.White;
            }
        }

        private void TxtBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBusca.Text))
            {
                TxtBusca.Text = "Buscar curso...";
                TxtBusca.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#9E8FC0"));
            }
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtBusca.Text != "Buscar curso...") AplicarFiltro();
        }
    }

    public class CursoMenuVM
    {
        public int CursoId { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string NomeInstrutor { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public string? DescricaoCategoria { get; set; }
        public string CargaHoraria { get; set; } = "";
        public string NumAlunos { get; set; } = "";
        public string Preco { get; set; } = "";
        public SolidColorBrush CorFundoCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public bool BotaoAtivo { get; set; }
        public string TextoBotao { get; set; } = "";
        public SolidColorBrush CorBotao { get; set; } = new();
    }
}
