using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;

namespace Learnix
{
    public partial class TelaEscolherCurso : UserControl
    {
        private int _matriculaCorrecaoAtualId;

        private Instrutor? _instrutor;

        private List<TextBox> _caixasDeNota = new List<TextBox>();

        private ObservableCollection<AulaTempVM> _aulasTemporarias = new ObservableCollection<AulaTempVM>();

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
            using var db = new LearnixDbContext();
            var cats = db.Categorias.ToList();
            ComboCategorias.ItemsSource = cats;
            ComboCategorias.DisplayMemberPath = "Nome";
            ComboCategorias.SelectedValuePath = "Id";
            if (cats.Any()) ComboCategorias.SelectedIndex = 0;
        }

        private void CarregarCursos()
        {
            using var db = new LearnixDbContext();

            var cursos = db.Cursos
                .Where(c => c.InstrutorId == null)
                .Include(c => c.Categoria)
                .ToList();

            if (cursos.Count == 0)
            {
                PainelVazio.Visibility = Visibility.Visible;
                ListaCursos.Visibility = Visibility.Collapsed;
                return;
            }

            PainelVazio.Visibility = Visibility.Collapsed;
            ListaCursos.Visibility = Visibility.Visible;

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

        // ── Botão Adicionar Curso ────────────────────────────────────────────

        private void BtnAdicionarCurso_Click(object sender, RoutedEventArgs e)
        {
            PainelNovoCurso.Visibility = Visibility.Visible;
            TxtTitulo.Clear();
            TxtDescricao.Clear();
            TxtCargaHoraria.Clear();
            TxtPreco.Clear();

            // Limpa as áreas de aula
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
                MessageBox.Show("Informe o título e a duração da aula.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(duracaoStr, out int duracaoMinutos) || duracaoMinutos <= 0)
            {
                MessageBox.Show("A duração deve ser um número inteiro positivo.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Trava se o instrutor não tiver selecionado o vídeo
            if (string.IsNullOrEmpty(caminhoVideo))
            {
                MessageBox.Show("Por favor, selecione o arquivo de vídeo clicando no ícone de pasta.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (sender is Button btn && btn.Tag is string idTemp)
            {
                var aula = _aulasTemporarias.FirstOrDefault(a => a.IdTemp == idTemp);
                if (aula != null)
                {
                    _aulasTemporarias.Remove(aula);

                    // Reordena os itens na interface (1, 2, 3...)
                    for (int i = 0; i < _aulasTemporarias.Count; i++)
                    {
                        _aulasTemporarias[i].Ordem = i + 1;
                    }

                    // Atualiza interface
                    ListaAulasNovas.ItemsSource = null;
                    ListaAulasNovas.ItemsSource = _aulasTemporarias;
                }
            }
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
                MessageBox.Show("Para salvar, é obrigatório preencher o enunciado das 3 atividades avaliativas discursivas.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descricao) ||
                string.IsNullOrEmpty(cargaStr))
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

            // Determina o tipo pelo nome da categoria
            using var db = new LearnixDbContext();
            var categoria = db.Categorias.Find(categoriaId);
            string nomeCategoria = categoria?.Nome ?? "Exatas";

            Curso novoCurso = new Curso();

            novoCurso.Titulo = titulo;
            novoCurso.Descricao = descricao;
            novoCurso.CargaHoraria = carga;
            novoCurso.Preco = preco;
            novoCurso.CategoriaId = categoriaId;
            // InstrutorId null = disponível para candidatura

            if (_aulasTemporarias.Any())
            {
                var moduloUnico = new Modulo
                {
                    Titulo = "Módulo 1",
                    Ordem = 1,
                    Aulas = _aulasTemporarias.Select(a => new Aula
                    {
                        Titulo = a.Titulo,
                        VideoUrl = a.Url,
                        Duracao = TimeSpan.FromMinutes(a.Duracao),
                        Ordem = a.Ordem
                    }).ToList()
                };
                novoCurso.Modulos.Add(moduloUnico);
            }

            novoCurso.Atividades = new List<AtividadeCurso>
            {
                new AtividadeCurso { Pergunta = p1 },
                new AtividadeCurso { Pergunta = p2 },
                new AtividadeCurso { Pergunta = p3 }
            };

            db.Cursos.Add(novoCurso);
            db.SaveChanges();

            MessageBox.Show($"Curso \"{titulo}\" criado com sucesso!\nEle já está disponível para candidatura.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            PainelNovoCurso.Visibility = Visibility.Collapsed;
            CarregarCursos();
        }


        private void BtnCandidatar_Click(object sender, RoutedEventArgs e)
        {
            if (_instrutor == null || sender is not Button btn || btn.Tag is not int cursoId) return;

            using var db = new LearnixDbContext();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == cursoId);
            if (curso == null) return;

            curso.InstrutorId = _instrutor.Id;
            db.SaveChanges();

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
                MessageBox.Show("O nome da categoria é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new LearnixDbContext())
            {
                if (db.Categorias.Any(c => c.Nome.ToLower() == nome.ToLower()))
                {
                    MessageBox.Show("Já existe uma categoria cadastrada com este nome.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var novaCategoria = new Categoria
                {
                    Nome = nome,
                    Descricao = descricao
                };

                db.Categorias.Add(novaCategoria);
                db.SaveChanges();
            }

            MessageBox.Show($"Categoria '{nome}' adicionada com sucesso!\nEla já pode ser selecionada ao criar um novo curso.", "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            PainelNovaCategoria.Visibility = Visibility.Collapsed;
        }

        private void BtnSelecionarVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Selecionar Vídeo da Aula",
                Filter = "Arquivos de Vídeo (*.mp4;*.avi;*.mkv)|*.mp4;*.avi;*.mkv|Todos os Arquivos (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TxtCaminhoVideo.Text = openFileDialog.FileName;
            }
        }

        public void AbrirCorrecao(int matriculaIdAguardando)
        {
            using var db = new LearnixDbContext();
            var matricula = db.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                .FirstOrDefault(m => m.Id == matriculaIdAguardando);

            var respostas = db.RespostasAtividades
                .Include(r => r.AtividadeCurso)
                .Where(r => r.MatriculaId == matriculaIdAguardando)
                .ToList();

            if (respostas.Count < 3) return;

            _matriculaCorrecaoAtualId = matriculaIdAguardando;
            TxtNomeAlunoCorrecao.Text = $"Aluno: {matricula?.Aluno.Nome} | Curso: {matricula?.Curso.Titulo}";
            ListaRespostasParaCorrigir.Children.Clear();
            _caixasDeNota.Clear();

            for (int i = 0; i < respostas.Count; i++)
            {
                var r = respostas[i];

                ListaRespostasParaCorrigir.Children.Add(new TextBlock { Text = $"Q{i + 1}: {r.AtividadeCurso.Pergunta}", Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8CCF0")), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 4) });

                ListaRespostasParaCorrigir.Children.Add(new TextBlock { Text = $"Resposta: {r.Resposta}", Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 8) });

                ListaRespostasParaCorrigir.Children.Add(new TextBlock { Text = "Nota (0 a 10):", Foreground = Brushes.Yellow, FontSize = 11 });
                var txtNota = new TextBox { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2860")), Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 20), Padding = new Thickness(8), Width = 60, HorizontalAlignment = HorizontalAlignment.Left, Tag = r.Id };

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
            using var db = new LearnixDbContext();
            var matricula = db.Matriculas.Include(m => m.Curso).FirstOrDefault(m => m.Id == _matriculaCorrecaoAtualId);
            if (matricula == null) return;

            decimal somaNotas = 0;

            foreach (var txt in _caixasDeNota)
            {
                if (!decimal.TryParse(txt.Text, out decimal notaLida) || notaLida < 0 || notaLida > 10)
                {
                    MessageBox.Show("Preencha todas as notas com valores válidos entre 0 e 10.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int respostaId = (int)txt.Tag;
                var respostaBanco = db.RespostasAtividades.Find(respostaId);
                if (respostaBanco != null)
                {
                    respostaBanco.Nota = notaLida;
                    somaNotas += notaLida;
                }
            }

            decimal media = somaNotas / 3;

            if (media >= 7.0m)
            {
                matricula.Status = StatusMatricula.Concluida;

                db.Certificados.Add(new Certificado
                {
                    MatriculaId = matricula.Id,
                    CodigoCertificado = "LX-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                    DataEmissao = DateTime.Now
                });

                MessageBox.Show($"Avaliação salva! O aluno foi APROVADO com média {media:F1} e o certificado foi emitido.", "Aprovado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                matricula.Status = StatusMatricula.Reprovada;
                MessageBox.Show($"O aluno foi REPROVADO com média {media:F1}. A matrícula foi cancelada e ele precisará refazer o curso do zero.", "Reprovado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            db.SaveChanges();
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
