using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaEscolherCurso : UserControl
    {
        private readonly CursoController _cursoController = new();
        private readonly CategoriaController _categoriaController = new();
        private readonly MatriculaController _matriculaController = new();
        private readonly AvaliacaoController _avaliacaoController = new();

        private int _matriculaCorrecaoAtualId;
        private Instrutor? _instrutor;
        private List<TextBox> _caixasDeNota = new();
        private ObservableCollection<AulaTempVM> _aulasTemporarias = new();

        public TelaEscolherCurso()
        {
            InitializeComponent();
        }

        public void DefinirInstrutor(Instrutor instrutor)
        {
            _instrutor = instrutor;
            Sidebar.DefinirInstrutor(instrutor.Nome);
            CarregarCategorias();
            CarregarCursos();
        }

        private void CarregarCategorias()
        {
            var cats = _categoriaController.ListarTodas();
            ComboCategorias.ItemsSource = cats;
            ComboCategorias.DisplayMemberPath = "Nome";
            ComboCategorias.SelectedValuePath = "Id";
            if (cats.Any()) ComboCategorias.SelectedIndex = 0;
        }

        private void CarregarCursos()
        {
            var cursos = _cursoController.ListarSemInstrutor();

            PainelVazio.Visibility = cursos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaCursos.Visibility = cursos.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (cursos.Count == 0) return;

            ListaCursos.ItemsSource = cursos.Select(c =>
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

                return new EscolherCursoVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    NomeCategoria = categoria,
                    CargaHoraria = $"{c.CargaHoraria}h",
                    CorCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    TextoBotao = "Candidatar-se",
                    CorBotao = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A")),
                    BotaoAtivo = true,
                };
            }).ToList();
        }

        private void BtnAdicionarCurso_Click(object sender, RoutedEventArgs e)
        {
            PainelNovoCurso.Visibility = Visibility.Visible;
            TxtTitulo.Clear();
            TxtDescricao.Clear();
            TxtCargaHoraria.Clear();
            TxtPreco.Clear();
            TxtAulaTitulo.Clear();
            TxtAulaDuracao.Clear();
            TxtCaminhoVideo.Clear();
            _aulasTemporarias.Clear();
            TxtPergunta1.Clear();
            TxtPergunta2.Clear();
            TxtPergunta3.Clear();
            ListaAulasNovas.ItemsSource = _aulasTemporarias;
        }

        private void BtnAddAula_Click(object sender, RoutedEventArgs e)
        {
            string titulo = TxtAulaTitulo.Text.Trim();
            string duracaoStr = TxtAulaDuracao.Text.Trim();
            string caminhoVideo = TxtCaminhoVideo.Text.Trim();

            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(duracaoStr))
            {
                MessageBox.Show("Informe o título e a duração da aula.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(duracaoStr, out int duracaoMinutos) || duracaoMinutos <= 0)
            {
                MessageBox.Show("A duração deve ser um número inteiro positivo.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(caminhoVideo))
            {
                MessageBox.Show("Por favor, selecione o arquivo de vídeo clicando no ícone de pasta.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _aulasTemporarias.Add(new AulaTempVM
            {
                Ordem = _aulasTemporarias.Count + 1,
                Titulo = titulo,
                Duracao = duracaoMinutos,
                Url = caminhoVideo
            });

            TxtAulaTitulo.Clear();
            TxtAulaDuracao.Clear();
            TxtCaminhoVideo.Clear();
        }

        private void BtnRemoverAula_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not string idTemp) return;

            var aula = _aulasTemporarias.FirstOrDefault(a => a.IdTemp == idTemp);
            if (aula == null) return;

            _aulasTemporarias.Remove(aula);

            for (int i = 0; i < _aulasTemporarias.Count; i++)
                _aulasTemporarias[i].Ordem = i + 1;

            ListaAulasNovas.ItemsSource = null;
            ListaAulasNovas.ItemsSource = _aulasTemporarias;
        }

        private void BtnSalvarNovoCurso_Click(object sender, RoutedEventArgs e)
        {
            string titulo = TxtTitulo.Text.Trim();
            string descricao = TxtDescricao.Text.Trim();
            string cargaStr = TxtCargaHoraria.Text.Trim();
            string precoStr = TxtPreco.Text.Trim();
            string p1 = TxtPergunta1.Text.Trim();
            string p2 = TxtPergunta2.Text.Trim();
            string p3 = TxtPergunta3.Text.Trim();

            if (string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(p2) || string.IsNullOrEmpty(p3))
            {
                MessageBox.Show("Para salvar, é obrigatório preencher o enunciado das 3 atividades avaliativas discursivas.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descricao) || string.IsNullOrEmpty(cargaStr))
            {
                MessageBox.Show("Preencha pelo menos Título, Descrição e Carga Horária.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(cargaStr, out int carga) || carga <= 0)
            {
                MessageBox.Show("Carga horária deve ser um número inteiro positivo.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(precoStr.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal preco))
                preco = 0;

            var categoriaId = ComboCategorias.SelectedValue is int id ? id : 1;

            var novoCurso = new Curso
            {
                Titulo = titulo,
                Descricao = descricao,
                CargaHoraria = carga,
                Preco = preco,
                CategoriaId = categoriaId,
            };

            if (_aulasTemporarias.Any())
            {
                novoCurso.Modulos.Add(new Modulo
                {
                    Titulo = "Módulo 1",
                    Ordem = 1,
                    Aulas = _aulasTemporarias.Select(a => new Aula
                    {
                        Titulo = a.Titulo,
                        VideoUrl = a.Url,
                        Duracao = TimeSpan.FromMinutes(a.Duracao),
                        Ordem = a.Ordem,
                    }).ToList()
                });
            }

            novoCurso.Atividades = new List<AtividadeCurso>
            {
                new AtividadeCurso { Pergunta = p1 },
                new AtividadeCurso { Pergunta = p2 },
                new AtividadeCurso { Pergunta = p3 },
            };

            _cursoController.Adicionar(novoCurso);

            MessageBox.Show($"Curso \"{titulo}\" criado com sucesso!\nEle já está disponível para candidatura.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            PainelNovoCurso.Visibility = Visibility.Collapsed;
            CarregarCursos();
        }

        private void BtnCandidatar_Click(object sender, RoutedEventArgs e)
        {
            if (_instrutor == null || sender is not Button btn || btn.Tag is not int cursoId) return;

            var cursos = _cursoController.ListarSemInstrutor();
            var curso = cursos.FirstOrDefault(c => c.Id == cursoId);
            if (curso == null) return;

            _cursoController.VincularInstrutor(cursoId, _instrutor.Id);

            MessageBox.Show($"Você foi vinculado ao curso \"{curso.Titulo}\" com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregarCursos();
        }

        private void BtnCancelarNovoCurso_Click(object sender, RoutedEventArgs e)
        {
            PainelNovoCurso.Visibility = Visibility.Collapsed;
        }

        private void BtnNovaCategoria_Click(object sender, RoutedEventArgs e)
        {
            PainelNovaCategoria.Visibility = Visibility.Visible;
            TxtCategoriaNome.Clear();
            TxtCategoriaDescricao.Clear();
        }

        private void BtnCancelarCategoria_Click(object sender, RoutedEventArgs e)
        {
            PainelNovaCategoria.Visibility = Visibility.Collapsed;
        }

        private void BtnSalvarCategoria_Click(object sender, RoutedEventArgs e)
        {
            string nome = TxtCategoriaNome.Text.Trim();
            string descricao = TxtCategoriaDescricao.Text.Trim();

            if (string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("O nome da categoria é obrigatório.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool adicionada = _categoriaController.Adicionar(nome, descricao);
            if (!adicionada)
            {
                MessageBox.Show("Já existe uma categoria cadastrada com este nome.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Categoria '{nome}' adicionada com sucesso!\nEla já pode ser selecionada ao criar um novo curso.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            PainelNovaCategoria.Visibility = Visibility.Collapsed;
        }

        private void BtnSelecionarVideo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Selecionar Vídeo da Aula",
                Filter = "Arquivos de Vídeo (*.mp4;*.avi;*.mkv)|*.mp4;*.avi;*.mkv|Todos os Arquivos (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
                TxtCaminhoVideo.Text = dlg.FileName;
        }

        public void AbrirCorrecao(int matriculaIdAguardando)
        {
            var matricula = _matriculaController.BuscarCompleta(matriculaIdAguardando);
            var respostas = _avaliacaoController.ListarRespostas(matriculaIdAguardando);

            if (respostas.Count < 3) return;

            _matriculaCorrecaoAtualId = matriculaIdAguardando;
            TxtNomeAlunoCorrecao.Text = $"Aluno: {matricula?.Aluno?.Nome} | Curso: {matricula?.Curso?.Titulo}";
            ListaRespostasParaCorrigir.Children.Clear();
            _caixasDeNota.Clear();

            for (int i = 0; i < respostas.Count; i++)
            {
                var r = respostas[i];

                ListaRespostasParaCorrigir.Children.Add(new TextBlock
                {
                    Text = $"Q{i + 1}: {r.AtividadeCurso.Pergunta}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8CCF0")),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 4),
                });
                ListaRespostasParaCorrigir.Children.Add(new TextBlock
                {
                    Text = $"Resposta: {r.Resposta}",
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 8),
                });
                ListaRespostasParaCorrigir.Children.Add(new TextBlock
                {
                    Text = "Nota (0 a 10):",
                    Foreground = Brushes.Yellow,
                    FontSize = 11,
                });

                var txtNota = new TextBox
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2860")),
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 20),
                    Padding = new Thickness(8),
                    Width = 60,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = r.Id,
                };
                _caixasDeNota.Add(txtNota);
                ListaRespostasParaCorrigir.Children.Add(txtNota);
            }

            PainelCorrecao.Visibility = Visibility.Visible;
        }

        private void BtnFecharCorrecao_Click(object sender, RoutedEventArgs e)
        {
            PainelCorrecao.Visibility = Visibility.Collapsed;
        }

        private void BtnSalvarNotasProfessor_Click(object sender, RoutedEventArgs e)
        {
            var notas = new Dictionary<int, decimal>();

            foreach (var txt in _caixasDeNota)
            {
                if (!decimal.TryParse(txt.Text, out decimal notaLida) || notaLida < 0 || notaLida > 10)
                {
                    MessageBox.Show("Preencha todas as notas com valores válidos entre 0 e 10.", "Atenção",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                notas[(int)txt.Tag] = notaLida;
            }

            var (aprovado, media) = _avaliacaoController.SalvarNotas(_matriculaCorrecaoAtualId, notas);

            if (aprovado)
                MessageBox.Show($"Avaliação salva! O aluno foi APROVADO com média {media:F1} e o certificado foi emitido.",
                    "Aprovado", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"O aluno foi REPROVADO com média {media:F1}. A matrícula foi cancelada e ele precisará refazer o curso do zero.",
                    "Reprovado", MessageBoxButton.OK, MessageBoxImage.Warning);

            PainelCorrecao.Visibility = Visibility.Collapsed;
        }
    }

    public class EscolherCursoVM
    {
        public int CursoId { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public SolidColorBrush CorCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public string TextoBotao { get; set; } = "";
        public SolidColorBrush CorBotao { get; set; } = new();
        public bool BotaoAtivo { get; set; } = true;
    }

    public class AulaTempVM
    {
        public string IdTemp { get; set; } = Guid.NewGuid().ToString();
        public int Ordem { get; set; }
        public string Titulo { get; set; } = "";
        public int Duracao { get; set; }
        public string Url { get; set; } = "";
    }
}
