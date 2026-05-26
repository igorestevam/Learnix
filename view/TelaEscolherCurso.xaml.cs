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
            CarregarCursos();
        }

        private void CarregarCursos()
        {
            using var db = new LearnixDbContext();

            // Cursos sem instrutor vinculado
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

        private void BtnCandidatar_Click(object sender, RoutedEventArgs e)
        {
            if (_instrutor == null || sender is not Button btn || btn.Tag is not int cursoId) return;

            using var db = new LearnixDbContext();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == cursoId);
            if (curso == null) return;

            curso.InstrutorId = _instrutor.Id;
            db.SaveChanges();

            btn.Content = "✔ Vinculado";
            btn.IsEnabled = false;

            MessageBox.Show($"Você foi vinculado ao curso \"{curso.Titulo}\" com sucesso!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            // Recarrega para remover o curso da lista
            CarregarCursos();
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
}