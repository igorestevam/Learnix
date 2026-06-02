using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Learnix.control;
using Learnix.model;

namespace Learnix
{
    public partial class TelaHomeInstrutor : UserControl
    {
        private readonly CursoController _cursoController = new();

        public TelaHomeInstrutor()
        {
            InitializeComponent();
        }

        public void DefinirInstrutor(Instrutor instrutor)
        {
            TxtNome.Text = instrutor.Nome;
            TxtEspecialidade.Text = instrutor.Especialidade;

            if (!string.IsNullOrWhiteSpace(instrutor.Biografia))
            {
                TxtBiografia.Text = instrutor.Biografia;
                TxtBiografia.Visibility = System.Windows.Visibility.Visible;
            }

            Sidebar.DefinirInstrutor(instrutor.Nome);
            CarregarDados(instrutor.Id);
        }

        private void CarregarDados(int instrutorId)
        {
            var cursos = _cursoController.ListarPorInstrutor(instrutorId);

            TxtTotalCursos.Text = cursos.Count.ToString();
            TxtTotalAlunos.Text = cursos.Sum(c => c.MatriculasAtivas?.Count ?? 0).ToString();

            PainelVazio.Visibility = cursos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ListaCursos.Visibility = cursos.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (cursos.Count == 0) return;

            ListaCursos.ItemsSource = cursos.Select(c => new
            {
                Titulo = c.Titulo,
                Info = $"{c.Categoria?.Nome ?? "Geral"}  •  {c.CargaHoraria}h",
                NumAlunos = $"👥 {c.MatriculasAtivas?.Count ?? 0} alunos",
            }).ToList();
        }
    }
}
