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

        private int _matriculaCorrecaoAtualId;
        private Instrutor? _instrutor;
        private List<RespostaCorrecaoVM> _respostasAtual = new();
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
            if (sender is Button btn && btn.Tag is int cursoId)
            {
                using var db = new LearnixDbContext();

                var pendentes = db.Matriculas
                    .Include(m => m.Aluno)
                    .Where(m => m.CursoId == cursoId && m.Status == StatusMatricula.AguardandoCorrecao)
                    .Select(m => new AlunoPendenteVM
                    {
                        MatriculaId = m.Id,
                        AlunoNome = m.Aluno != null ? m.Aluno.Nome : "Aluno Desconhecido"
                    }).ToList();

                ListaAlunosPendentes.ItemsSource = pendentes;
                TxtSemAlunos.Visibility = pendentes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                PainelAlunosPendentes.Visibility = Visibility.Visible;
            }
        }

        private void BtnFecharListaPendentes_Click(object sender, RoutedEventArgs e)
        {
            PainelAlunosPendentes.Visibility = Visibility.Collapsed;
        }

        private void BtnAbrirCorrecaoAluno_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int matriculaId)
            {
                using var db = new LearnixDbContext();
                var matricula = db.Matriculas.Include(m => m.Aluno).FirstOrDefault(m => m.Id == matriculaId);
                var respostas = db.RespostasAtividades.Include(r => r.AtividadeCurso).Where(r => r.MatriculaId == matriculaId).ToList();

                if (matricula == null || respostas.Count == 0) return;

                _matriculaCorrecaoAtualId = matriculaId;
                TxtNomeAlunoCorrecao.Text = $"Aluno: {matricula.Aluno?.Nome}";

                _respostasAtual.Clear();
                for (int i = 0; i < respostas.Count; i++)
                {
                    _respostasAtual.Add(new RespostaCorrecaoVM
                    {
                        RespostaId = respostas[i].Id,
                        NumeroPergunta = i + 1,
                        Pergunta = respostas[i].AtividadeCurso?.Pergunta ?? "",
                        Resposta = respostas[i].Resposta ?? "",
                        NotaDigitada = ""
                    });
                }

                ListaRespostasParaCorrigir.ItemsSource = null;
                ListaRespostasParaCorrigir.ItemsSource = _respostasAtual;

                PainelAlunosPendentes.Visibility = Visibility.Collapsed;
                PainelCorrecao.Visibility = Visibility.Visible;
            }
        }

        private void BtnFecharCorrecao_Click(object sender, RoutedEventArgs e)
        {
            PainelCorrecao.Visibility = Visibility.Collapsed;
        }

        private void BtnSalvarNotasProfessor_Click(object sender, RoutedEventArgs e)
        {
            using var db = new LearnixDbContext();
            var matricula = db.Matriculas.FirstOrDefault(m => m.Id == _matriculaCorrecaoAtualId);
            if (matricula == null) return;

            decimal somaNotas = 0;

            foreach (var rVM in _respostasAtual)
            {
                if (!decimal.TryParse(rVM.NotaDigitada.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal notaLida) || notaLida < 0 || notaLida > 10)
                {
                    MessageBox.Show("Preencha todas as notas com valores válidos numéricos entre 0 e 10.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var respostaBanco = db.RespostasAtividades.Find(rVM.RespostaId);
                if (respostaBanco != null)
                {
                    respostaBanco.Nota = notaLida;
                    somaNotas += notaLida;
                }
            }

            decimal media = somaNotas / _respostasAtual.Count;

            if (media >= 7.0m)
            {
                matricula.Status = StatusMatricula.Concluida;

                var certificadoExistente = db.Certificados.FirstOrDefault(c => c.MatriculaId == matricula.Id);

                if (certificadoExistente == null)
                {
                    db.Certificados.Add(new Certificado
                    {
                        MatriculaId = matricula.Id,
                        CodigoCertificado = "LX-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                        DataEmissao = DateTime.Now
                    });
                }

                MessageBox.Show($"Avaliação salva! O aluno foi APROVADO com média {media:F1}.", "Aprovado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                matricula.Status = StatusMatricula.Reprovada;
                MessageBox.Show($"O aluno foi REPROVADO com média {media:F1}. O status foi alterado para reprovado.", "Reprovado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            db.SaveChanges();
            PainelCorrecao.Visibility = Visibility.Collapsed;
            CarregarCursos();
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
    public class AlunoPendenteVM
    {
        public int MatriculaId { get; set; }
        public string AlunoNome { get; set; } = "";
    }
    public class RespostaCorrecaoVM
    {
        public int RespostaId { get; set; }
        public int NumeroPergunta { get; set; }
        public string Pergunta { get; set; } = "";
        public string Resposta { get; set; } = "";
        public string NotaDigitada { get; set; } = "";
    }
}