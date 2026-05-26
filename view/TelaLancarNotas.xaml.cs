using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaLancarNotas : UserControl
    {
        private Instrutor? _instrutor;
        private List<AlunoNotaVM> _alunosVM = new();

        public TelaLancarNotas()
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
                .ToList();


            ComboCursos.SelectedValuePath = "Id";
            ComboCursos.ItemsSource = cursos;

            if (cursos.Any())
                ComboCursos.SelectedIndex = 0;
        }

        private void ComboCursos_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void BtnCarregar_Click(object sender, RoutedEventArgs e)
        {
            if (ComboCursos.SelectedItem is not Curso curso) return;

            using var db = new LearnixDbContext();
            var matriculas = db.Matriculas
                .Where(m => m.CursoId == curso.Id)
                .Include(m => m.Aluno)
                .Include(m => m.Avaliacoes)
                .ToList();

            if (matriculas.Count == 0)
            {
                PainelNotas.Visibility = Visibility.Visible;
                PainelSemAlunos.Visibility = Visibility.Visible;
                ListaAlunos.Visibility = Visibility.Collapsed;
                return;
            }

            PainelNotas.Visibility = Visibility.Visible;
            PainelSemAlunos.Visibility = Visibility.Collapsed;
            ListaAlunos.Visibility = Visibility.Visible;

            _alunosVM = matriculas.Select(m =>
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

            ListaAlunos.ItemsSource = _alunosVM;
        }

        private void BtnSalvarNota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int matriculaId) return;

            var item = _alunosVM.FirstOrDefault(a => a.MatriculaId == matriculaId);
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
    }

    public class AlunoNotaVM : INotifyPropertyChanged
    {
        public int MatriculaId { get; set; }
        public string NomeAluno { get; set; } = "";
        public string Matricula { get; set; } = "";

        private string _notaAV1 = "";
        private string _notaAV2 = "";
        private string _notaAV3 = "";

        public string NotaAV1
        {
            get => _notaAV1;
            set { _notaAV1 = value; OnPropertyChanged(nameof(NotaAV1)); }
        }
        public string NotaAV2
        {
            get => _notaAV2;
            set { _notaAV2 = value; OnPropertyChanged(nameof(NotaAV2)); }
        }
        public string NotaAV3
        {
            get => _notaAV3;
            set { _notaAV3 = value; OnPropertyChanged(nameof(NotaAV3)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}