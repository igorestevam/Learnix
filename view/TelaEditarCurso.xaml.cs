using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.control;
using Learnix.model;
using Microsoft.Win32;

namespace Learnix
{
    public partial class TelaEditarCurso : UserControl
    {
        private readonly CursoController _cursoController = new();

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

            _curso = _cursoController.BuscarPorId(curso.Id) ?? curso;

            TxtTitulo.Text = _curso.Titulo;
            TxtDescricao.Text = _curso.Descricao;
            TxtCargaHoraria.Text = _curso.CargaHoraria.ToString();

            CarregarModulos();
            CarregarAtividades();
        }

        private void CarregarModulos()
        {
            if (_curso == null) return;

            var modulos = _curso.Modulos?.OrderBy(m => m.Ordem).ToList()
                          ?? new List<Modulo>();

            PainelSemModulos.Visibility = modulos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaModulos.Visibility = modulos.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

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

        private void CarregarAtividades()
        {
            if (_curso == null) return;

            var atividades = _cursoController.ListarAtividades(_curso.Id);

            TxtPergunta1.Text = atividades.ElementAtOrDefault(0)?.Pergunta ?? "";
            TxtPergunta2.Text = atividades.ElementAtOrDefault(1)?.Pergunta ?? "";
            TxtPergunta3.Text = atividades.ElementAtOrDefault(2)?.Pergunta ?? "";
        }

        private void RecarregarCurso()
        {
            _curso = _cursoController.BuscarPorId(_curso!.Id) ?? _curso;
            CarregarModulos();
        }

        private void BtnSalvarAtividades_Click(object sender, RoutedEventArgs e)
        {
            if (_curso == null) return;

            var perguntas = new[]
            {
                TxtPergunta1.Text.Trim(),
                TxtPergunta2.Text.Trim(),
                TxtPergunta3.Text.Trim(),
            };

            if (perguntas.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Preencha as 3 perguntas antes de salvar.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _cursoController.SalvarAtividades(_curso.Id, perguntas);

            MessageBox.Show("Atividades salvas com sucesso!", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);
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

            string titulo = TxtTitulo.Text.Trim();
            string descricao = TxtDescricao.Text.Trim();

            _cursoController.AtualizarDados(_curso.Id, titulo, descricao, carga);

            _curso.Titulo = titulo;
            _curso.Descricao = descricao;
            _curso.CargaHoraria = carga;

            MessageBox.Show("Curso atualizado com sucesso!", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdicionarModulo_Click(object sender, RoutedEventArgs e)
        {
            if (_curso == null) return;

            var dialog = new InputDialog("Novo Módulo", "Nome do módulo:");
            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.Resposta)) return;

            _cursoController.AdicionarModulo(_curso.Id, dialog.Resposta.Trim());
            RecarregarCurso();
        }

        private void BtnRemoverModulo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int moduloId) return;

            var r = MessageBox.Show("Remover este módulo e todas as suas aulas?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;

            _cursoController.RemoverModulo(moduloId);
            RecarregarCurso();
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

            _cursoController.AdicionarAula(moduloId, dialogTitulo.Resposta.Trim(), minutos);
            RecarregarCurso();
        }

        private void BtnRemoverAula_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int aulaId) return;

            var r = MessageBox.Show("Remover esta aula?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r != MessageBoxResult.Yes) return;

            _cursoController.RemoverAula(aulaId);
            RecarregarCurso();
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

            _cursoController.AtualizarVideoAula(aulaId, dlg.FileName);
            RecarregarCurso();

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
