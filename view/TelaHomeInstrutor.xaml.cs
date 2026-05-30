using System.Linq;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class TelaHomeInstrutor : UserControl
    {
        public TelaHomeInstrutor()
        {
            InitializeComponent();
        }

        public void DefinirInstrutor(Instrutor instrutor)
        {
            TxtNome.Text = instrutor.Nome;
            TxtEspecialidade.Text = instrutor.Especialidade;
            Sidebar.DefinirInstrutor(instrutor.Nome);
            CarregarDados(instrutor.Id);
        }

        private void CarregarDados(int instrutorId)
        {
            using var db = new LearnixDbContext();

            var cursos = db.Cursos
                .Where(c => c.InstrutorId == instrutorId)
                .Include(c => c.MatriculasAtivas)
                    .ThenInclude(m => m.Avaliacoes)
                .Include(c => c.Categoria)
                .ToList();

            TxtTotalCursos.Text = cursos.Count.ToString();

            int totalAlunos = cursos.Sum(c => c.MatriculasAtivas?.Count ?? 0);
            TxtTotalAlunos.Text = totalAlunos.ToString();

            int notasPendentes = cursos
                .SelectMany(c => c.MatriculasAtivas ?? new System.Collections.Generic.List<Matricula>())
                .Count(m => m.Avaliacoes == null || !m.Avaliacoes.Any());
           

            if (cursos.Count == 0)
            {
                PainelVazio.Visibility = System.Windows.Visibility.Visible;
                ListaCursos.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            PainelVazio.Visibility = System.Windows.Visibility.Collapsed;
            ListaCursos.Visibility = System.Windows.Visibility.Visible;

            ListaCursos.ItemsSource = cursos.Select(c => new
            {
                Titulo = c.Titulo,
                Info = $"{c.Categoria?.Nome ?? "Geral"}  •  {c.CargaHoraria}h",
                NumAlunos = $"👥 {c.MatriculasAtivas?.Count ?? 0} alunos",
            }).ToList();
        }
    }
}