using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class TelaMeusCursosInstrutor : UserControl
    {
        private Instrutor? _instrutor;
        private List<CursoInstrutorVM> _cursos = new();

        public TelaMeusCursosInstrutor()
        {
            InitializeComponent();
        }

        public void DefinirInstrutor(Instrutor instrutor)
        {
            _instrutor = instrutor;
            Sidebar.DefinirInstrutor(instrutor.Nome);
            CarregarCursos();
        }

        private void CarregarCursos()
        {
            if (_instrutor == null) return;

            using var db = new LearnixDbContext();

            var cursos = db.Cursos
                .Where(c => c.InstrutorId == _instrutor.Id)
                .Include(c => c.Categoria)
                .Include(c => c.MatriculasAtivas)
                .ToList();

            if (cursos.Count == 0)
            {
                PainelVazio.Visibility = Visibility.Visible;
                ListaCursos.Visibility = Visibility.Collapsed;
                return;
            }

            PainelVazio.Visibility = Visibility.Collapsed;
            ListaCursos.Visibility = Visibility.Visible;

            _cursos = cursos.Select(c =>
            {
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

                int numAlunos = c.MatriculasAtivas?.Count ?? 0;

                return new CursoInstrutorVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    NomeCategoria = categoria,
                    CargaHoraria = $"{c.CargaHoraria}h",
                    NumAlunos = $"👥 {numAlunos} aluno{(numAlunos == 1 ? "" : "s")}",
                    CorCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    PodeSair = numAlunos == 0,
                    PainelNotasVisivel = Visibility.Collapsed,
                    PainelSemAlunosVisivel = Visibility.Collapsed,
                };
            }).ToList();

            ListaCursos.ItemsSource = _cursos;
        }

        private void BtnEditarCurso_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int cursoId) return;
            if (_instrutor == null) return;

            using var db = new LearnixDbContext();
            var curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == cursoId);

            if (curso == null) return;

            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarEditarCurso(curso, _instrutor);
        }

        private void BtnLancarNotas_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int cursoId) return;

            var vm = _cursos.FirstOrDefault(c => c.CursoId == cursoId);
            if (vm == null) return;

            if (vm.PainelNotasVisivel == Visibility.Visible)
            {
                vm.PainelNotasVisivel = Visibility.Collapsed;
                return;
            }

            using var db = new LearnixDbContext();
            var matriculas = db.Matriculas
                .Where(m => m.CursoId == cursoId)
                .Include(m => m.Aluno)
                .Include(m => m.Avaliacoes)
                .ToList();

            vm.Alunos = matriculas.Select(m =>
            {
                var avs = m.Avaliacoes?.OrderBy(a => a.Titulo).ToList() ?? new List<Avaliacao>();
                return new AlunoNotaVM
                {
                    MatriculaId = m.Id,
                    NomeAluno = m.Aluno?.Nome ?? "Aluno",
                    Matricula = m.Aluno?.MatriculaAcademica ?? "",
                    NotaAV1 = avs.ElementAtOrDefault(0)?.Nota.ToString("0.0", CultureInfo.InvariantCulture) ?? "",
                    NotaAV2 = avs.ElementAtOrDefault(1)?.Nota.ToString("0.0", CultureInfo.InvariantCulture) ?? "",
                    NotaAV3 = avs.ElementAtOrDefault(2)?.Nota.ToString("0.0", CultureInfo.InvariantCulture) ?? "",
                };
            }).ToList();

            vm.PainelSemAlunosVisivel = matriculas.Count == 0
                ? Visibility.Visible : Visibility.Collapsed;
            vm.PainelNotasVisivel = Visibility.Visible;
        }

        private void BtnSalvarNota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            AlunoNotaVM? item = null;
            foreach (var curso in _cursos)
                item = curso.Alunos?.FirstOrDefault(a => a.MatriculaId == matriculaId) ?? item;

            if (item == null) return;

            var novasNotas = new (string Titulo, string Valor)[]
            {
                ("AV1", item.NotaAV1),
                ("AV2", item.NotaAV2),
                ("AV3", item.NotaAV3),
            };

            using var db = new LearnixDbContext();
            var avaliacoesExistentes = db.Avaliacoes
                .Where(a => a.MatriculaId == matriculaId)
                .ToList();

            foreach (var (titulo, valor) in novasNotas)
            {
                if (string.IsNullOrWhiteSpace(valor)) continue;

                if (!double.TryParse(valor.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out double nota)
                    || nota < 0 || nota > 10)
                {
                    MessageBox.Show($"{titulo}: valor inválido. Use um número entre 0 e 10.",
                        "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existente = avaliacoesExistentes.FirstOrDefault(a => a.Titulo == titulo);
                if (existente != null)
                {
                    existente.Nota = nota;
                    existente.DataRealizacao = DateTime.Now;
                }
                else
                {
                    db.Avaliacoes.Add(new Avaliacao
                    {
                        MatriculaId = matriculaId,
                        Titulo = titulo,
                        Nota = nota,
                        DataRealizacao = DateTime.Now,
                    });
                }
            }

            db.SaveChanges();
            MessageBox.Show($"Notas de {item.NomeAluno} salvas com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSairCurso_Click(object sender, RoutedEventArgs e)
        {
            if (_instrutor == null || sender is not Button btn || btn.Tag is not int cursoId) return;

            using var db = new LearnixDbContext();
            var curso = db.Cursos
                .Include(c => c.MatriculasAtivas)
                .FirstOrDefault(c => c.Id == cursoId);

            if (curso == null) return;

            int numAlunos = curso.MatriculasAtivas?.Count ?? 0;
            if (numAlunos > 0)
            {
                MessageBox.Show(
                    $"Não é possível sair do curso \"{curso.Titulo}\".\n\n" +
                    $"Existem {numAlunos} aluno(s) matriculado(s).",
                    "Ação Bloqueada", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var r = MessageBox.Show(
                $"Deseja realmente sair do curso \"{curso.Titulo}\"?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (r != MessageBoxResult.Yes) return;

            curso.InstrutorId = null;
            db.SaveChanges();

            MessageBox.Show($"Você saiu do curso \"{curso.Titulo}\" com sucesso.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregarCursos();
        }
    }

    public class CursoInstrutorVM : INotifyPropertyChanged
    {
        public int CursoId { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public string NumAlunos { get; set; } = "";
        public SolidColorBrush CorCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public bool PodeSair { get; set; } = true;
        public List<AlunoNotaVM>? Alunos { get; set; }

        private Visibility _painelNotasVisivel = Visibility.Collapsed;
        public Visibility PainelNotasVisivel
        {
            get => _painelNotasVisivel;
            set { _painelNotasVisivel = value; OnPropertyChanged(nameof(PainelNotasVisivel)); }
        }

        private Visibility _painelSemAlunosVisivel = Visibility.Collapsed;
        public Visibility PainelSemAlunosVisivel
        {
            get => _painelSemAlunosVisivel;
            set { _painelSemAlunosVisivel = value; OnPropertyChanged(nameof(PainelSemAlunosVisivel)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}