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

                return new MeuCursoInstrutorVM
                {
                    CursoId = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    NomeCategoria = categoria,
                    CargaHoraria = $"{c.CargaHoraria}h",
                    NumAlunos = $"👥 {c.MatriculasAtivas?.Count ?? 0} alunos",
                    CorCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corFundo)),
                    CorTextoCategoria = new SolidColorBrush((Color)ColorConverter.ConvertFromString(corTexto)),
                };
            }).ToList();
        }

        private void BtnLancarNotas_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            if (_instrutor != null)
                main?.MostrarLancarNotas(_instrutor);
        }
    }

    public class MeuCursoInstrutorVM
    {
        public int CursoId { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string NomeCategoria { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public string NumAlunos { get; set; } = "";
        public SolidColorBrush CorCategoria { get; set; } = new();
        public SolidColorBrush CorTextoCategoria { get; set; } = new();
    }
}