using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Learnix
{
    public partial class TelaEditarCurso : UserControl
    {
        private Instrutor? _instrutor;
        private Curso? _curso;

        public TelaEditarCurso()
        {
            InitializeComponent();
        }

        public void DefinirCurso(Curso curso, Instrutor instrutor)
        {
            _instrutor = instrutor;
            Sidebar.DefinirInstrutor(instrutor.Nome);

            using var db = new LearnixDbContext();
            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == curso.Id) ?? curso;

            TxtTitulo.Text = _curso.Titulo;
            TxtDescricao.Text = _curso.Descricao;
            TxtCargaHoraria.Text = _curso.CargaHoraria.ToString();

            CarregarModulos();
        }

        private void CarregarModulos()
        {
            if (_curso == null) return;

            var modulos = _curso.Modulos?.OrderBy(m => m.Ordem).ToList()
                          ?? new List<Modulo>();

            if (modulos.Count == 0)
            {
                PainelSemModulos.Visibility = Visibility.Visible;
                ListaModulos.Visibility = Visibility.Collapsed;
            }
            else
            {
                PainelSemModulos.Visibility = Visibility.Collapsed;
                ListaModulos.Visibility = Visibility.Visible;
            }

            int aulaGlobal = 1;
            ListaModulos.ItemsSource = modulos.Select(m => new ModuloEditVM
            {
                ModuloId = m.Id,
                TituloModulo = m.Titulo,
                Aulas = m.Aulas?.OrderBy(a => a.Ordem).Select(a => new AulaEditVM
                {
                    AulaId = a.Id,
                    NumeroAula = $"Aula {aulaGlobal++:D2}",
                    TituloAula = a.Titulo,
                    DuracaoAula = $"{a.Duracao.Minutes} min",
                    VideoUrl = a.VideoUrl ?? "",
                    VideoNome = string.IsNullOrWhiteSpace(a.VideoUrl)
                                    ? "Nenhum vídeo selecionado"
                                    : Path.GetFileName(a.VideoUrl),
                }).ToList() ?? new List<AulaEditVM>(),
            }).ToList();
        }

        private void BtnSalvarCurso_Click(object sender, RoutedEventArgs e)
        {
            if (_curso == null) return;

            if (string.IsNullOrWhiteSpace(TxtTitulo.Text))
            {
                MessageBox.Show("O título é obrigatório.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtCargaHoraria.Text.Trim(), out int carga) || carga <= 0)
            {
                MessageBox.Show("Carga horária inválida.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new LearnixDbContext();
            var curso = db.Cursos.Find(_curso.Id);
            if (curso == null) return;

            curso.Titulo = TxtTitulo.Text.Trim();
            curso.Descricao = TxtDescricao.Text.Trim();
            curso.CargaHoraria = carga;
            db.SaveChanges();

            _curso.Titulo = curso.Titulo;
            _curso.Descricao = curso.Descricao;
            _curso.CargaHoraria = curso.CargaHoraria;

            MessageBox.Show("Curso atualizado com sucesso!", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdicionarModulo_Click(object sender, RoutedEventArgs e)
        {
            if (_curso == null) return;

            var dialog = new InputDialog("Novo Módulo", "Nome do módulo:");
            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.Resposta)) return;

            using var db = new LearnixDbContext();
            var ordem = (db.Modulos
                .Where(m => m.CursoId == _curso.Id)
                .Max(m => (int?)m.Ordem) ?? 0) + 1;

            db.Modulos.Add(new Modulo
            {
                Titulo = dialog.Resposta.Trim(),
                Ordem = ordem,
                CursoId = _curso.Id,
            });
            db.SaveChanges();

            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == _curso.Id) ?? _curso;
            CarregarModulos();
        }

        private void BtnRemoverModulo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int moduloId) return;

            var r = MessageBox.Show("Remover este módulo e todas as suas aulas?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;

            using var db = new LearnixDbContext();
            var modulo = db.Modulos.Include(m => m.Aulas)
                .FirstOrDefault(m => m.Id == moduloId);
            if (modulo == null) return;

            db.Modulos.Remove(modulo);
            db.SaveChanges();

            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == _curso!.Id) ?? _curso;
            CarregarModulos();
        }

        private void BtnAdicionarAula_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int moduloId) return;

            var dialogTitulo = new InputDialog("Nova Aula", "Título da aula:");
            if (dialogTitulo.ShowDialog() != true || string.IsNullOrWhiteSpace(dialogTitulo.Resposta)) return;

            var dialogDuracao = new InputDialog("Duração", "Duração em minutos:");
            if (dialogDuracao.ShowDialog() != true) return;

            if (!int.TryParse(dialogDuracao.Resposta, out int minutos) || minutos <= 0)
                minutos = 30;

            using var db = new LearnixDbContext();
            var ordem = (db.Aulas
                .Where(a => a.ModuloId == moduloId)
                .Max(a => (int?)a.Ordem) ?? 0) + 1;

            db.Aulas.Add(new Aula
            {
                Titulo = dialogTitulo.Resposta.Trim(),
                VideoUrl = string.Empty,
                Duracao = System.TimeSpan.FromMinutes(minutos),
                Ordem = ordem,
                ModuloId = moduloId,
            });
            db.SaveChanges();

            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == _curso!.Id) ?? _curso;
            CarregarModulos();
        }

        private void BtnRemoverAula_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int aulaId) return;

            var r = MessageBox.Show("Remover esta aula?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;

            using var db = new LearnixDbContext();
            var aula = db.Aulas.Find(aulaId);
            if (aula == null) return;

            db.Aulas.Remove(aula);
            db.SaveChanges();

            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == _curso!.Id) ?? _curso;
            CarregarModulos();
        }

        private void BtnSalvarUrl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int aulaId) return;

            var dlg = new OpenFileDialog
            {
                Title = "Selecionar Vídeo da Aula",
                Filter = "Vídeos (*.mp4;*.wmv;*.avi)|*.mp4;*.wmv;*.avi|Todos (*.*)|*.*"
            };

            if (dlg.ShowDialog() != true) return;

            using var db = new LearnixDbContext();
            var aula = db.Aulas.Find(aulaId);
            if (aula == null) return;

            aula.VideoUrl = dlg.FileName;
            db.SaveChanges();

            _curso = db.Cursos
                .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
                .FirstOrDefault(c => c.Id == _curso!.Id) ?? _curso;
            CarregarModulos();

            MessageBox.Show("Vídeo vinculado com sucesso!", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            if (_instrutor != null)
                main?.MostrarMeusCursosInstrutor(_instrutor);
        }
    }

    public class ModuloEditVM
    {
        public int ModuloId { get; set; }
        public string TituloModulo { get; set; } = "";
        public List<AulaEditVM> Aulas { get; set; } = new();
    }

    public class AulaEditVM
    {
        public int AulaId { get; set; }
        public string NumeroAula { get; set; } = "";
        public string TituloAula { get; set; } = "";
        public string DuracaoAula { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public string VideoNome { get; set; } = "Nenhum vídeo selecionado";
    }
}