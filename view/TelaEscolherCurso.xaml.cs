using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaEscolherCurso : UserControl
    {
        private Instrutor? _instrutor;

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
            ComboCategorias.ItemsSource       = cats;
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
                string corFundo  = categoria switch
                {
                    "Humanas"    => "#1A3A2A",
                    "Tecnologia" => "#1A2A3A",
                    _            => "#3A2860"
                };
                string corTexto = categoria switch
                {
                    "Humanas"    => "#A5D6A7",
                    "Tecnologia" => "#90CAF9",
                    _            => "#D8CCF0"
                };

                return new EscolherCursoVM
                {
                    CursoId           = c.Id,
                    Titulo            = c.Titulo,
                    Descricao         = c.Descricao,
                    NomeCategoria     = categoria,
                    CargaHoraria      = $"{c.CargaHoraria}h",
                    CorCategoria      = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                    TextoBotao        = "Candidatar-se",
                    CorBotao          = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E3A7A")),
                    BotaoAtivo        = true,
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
        }

        private void BtnCancelarNovoCurso_Click(object sender, RoutedEventArgs e)
        {
            PainelNovoCurso.Visibility = Visibility.Collapsed;
        }

        private void BtnSalvarNovoCurso_Click(object sender, RoutedEventArgs e)
        {
            string titulo       = TxtTitulo.Text.Trim();
            string descricao    = TxtDescricao.Text.Trim();
            string cargaStr     = TxtCargaHoraria.Text.Trim();
            string precoStr     = TxtPreco.Text.Trim();

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

            Curso novoCurso = nomeCategoria == "Humanas"
                ? new CursoHumanas { ExigeMonografia = false }
                : new CursoExatas  { PossuiLaboratorioVirtual = false,
                                     FerramentaSoftwareSugerida = "" };

            novoCurso.Titulo       = titulo;
            novoCurso.Descricao    = descricao;
            novoCurso.CargaHoraria = carga;
            novoCurso.Preco        = preco;
            novoCurso.CategoriaId  = categoriaId;
            // InstrutorId null = disponível para candidatura

            db.Cursos.Add(novoCurso);
            db.SaveChanges();

            MessageBox.Show($"Curso \"{titulo}\" criado com sucesso!\nEle já está disponível para candidatura.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            PainelNovoCurso.Visibility = Visibility.Collapsed;
            CarregarCursos();
        }

        // ── Candidatar-se a curso existente ─────────────────────────────────

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
    }

    public class EscolherCursoVM
    {
        public int             CursoId           { get; set; }
        public string          Titulo            { get; set; } = "";
        public string          Descricao         { get; set; } = "";
        public string          NomeCategoria     { get; set; } = "";
        public string          CargaHoraria      { get; set; } = "";
        public SolidColorBrush CorCategoria      { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
        public string          TextoBotao        { get; set; } = "";
        public SolidColorBrush CorBotao          { get; set; } = new();
        public bool            BotaoAtivo        { get; set; } = true;
    }
}
