using System;
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

            using var db = new LearnixDbContext();

            var cursosNoBanco = db.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Instrutor)
                .Include(c => c.MatriculasAtivas)
                .ToList();

            var historicoAluno = db.Matriculas.Where(m => m.AlunoId == _aluno.Id).ToList();

            _todosCursos = cursosNoBanco.Select(c =>
            {
                var matricula = historicoAluno.FirstOrDefault(m => m.CursoId == c.Id);

                bool podeMatricular = matricula == null ||
                                      matricula.Status == StatusMatricula.Cancelada ||
                                      matricula.Status == StatusMatricula.Reprovada;

                string categoria = c.Categoria?.Nome ?? "Geral";
                string corFundo = categoria switch { "Humanas" => "#1A3A2A", "Tecnologia" => "#1A2A3A", _ => "#3A2860" };
                string corTexto = categoria switch { "Humanas" => "#A5D6A7", "Tecnologia" => "#90CAF9", _ => "#D8CCF0" };

                int numAlunos = c.MatriculasAtivas?.Count ?? 0;

                return new CursoMenuVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    NomeInstrutor = c.Instrutor != null ? $"Prof. {c.Instrutor.Nome}" : "Sem instrutor vinculado",
                    NomeCategoria = categoria,
                    CargaHoraria = $"🕐 {c.CargaHoraria}h",
                    NumAlunos = $"👥 {numAlunos} aluno(s)",
                    CorFundoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    BotaoAtivo = podeMatricular,
                    TextoBotao = podeMatricular ? "Matricular-se" : "Já Matriculado",
                    CorBotao = podeMatricular
                                            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"))
                                            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"))
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

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int cursoId) return;
            if (_aluno == null) return;

            var result = MessageBox.Show("Deseja confirmar a sua matrícula neste curso?",
                "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            using var db = new LearnixDbContext();

            var matriculaExistente = db.Matriculas
                .Include(m => m.Progresso)
                .FirstOrDefault(m => m.AlunoId == _aluno.Id && m.CursoId == cursoId);

            if (matriculaExistente != null)
            {
                if (matriculaExistente.Status == StatusMatricula.Cancelada ||
                    matriculaExistente.Status == StatusMatricula.Reprovada)
                {
                    matriculaExistente.Status = StatusMatricula.Ativa;
                    matriculaExistente.DataMatricula = DateTime.Now;

                    if (matriculaExistente.Progresso != null)
                        matriculaExistente.Progresso.PercentualConcluido = 0;
                    else
                        matriculaExistente.Progresso = new Progresso { PercentualConcluido = 0 };

                    var aulasAssistidas = db.AulasConcluidas
                        .Where(a => a.MatriculaId == matriculaExistente.Id).ToList();
                    if (aulasAssistidas.Any())
                        db.AulasConcluidas.RemoveRange(aulasAssistidas);

                    var respostasAntigas = db.RespostasAtividades
                        .Where(r => r.MatriculaId == matriculaExistente.Id).ToList();
                    if (respostasAntigas.Any())
                        db.RespostasAtividades.RemoveRange(respostasAntigas);

                    db.SaveChanges();
                    MessageBox.Show("Sua matrícula foi reativada! Você iniciará o curso do zero.",
                        "Bons Estudos", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                var novaMatricula = new Matricula
                {
                    AlunoId = _aluno.Id,
                    CursoId = cursoId,
                    Status = StatusMatricula.Ativa,
                    DataMatricula = DateTime.Now,
                    Progresso = new Progresso { PercentualConcluido = 0 }
                };
                db.Matriculas.Add(novaMatricula);
                db.SaveChanges();
                MessageBox.Show("Matrícula realizada com sucesso! Acesse 'Meus Cursos' para começar.",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }

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
        public string CargaHoraria { get; set; } = "";
        public string NumAlunos { get; set; } = "";
        public SolidColorBrush CorFundoCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public bool BotaoAtivo { get; set; }
        public string TextoBotao { get; set; } = "";
        public SolidColorBrush CorBotao { get; set; } = new();
    }
}