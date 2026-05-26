using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;
using Learnix.service;

namespace Learnix.view
{
    public partial class TelaMenu : UserControl
    {
        private Aluno? _aluno;
        private string _filtroAtivo = "Todos";

        public TelaMenu(Aluno aluno)
        {
            InitializeComponent();
            _aluno = aluno;
            Sidebar.DefinirUsuario(aluno);
            AtualizarBotoesFiltro();
        }

        private void AtualizarBotoesFiltro()
        {
            BtnTodos.Tag = (_filtroAtivo == "Todos") ? "ativo" : null;
            BtnExatas.Tag = (_filtroAtivo == "Exatas") ? "ativo" : null;
            BtnHumanas.Tag = (_filtroAtivo == "Humanas") ? "ativo" : null;
            BtnTecnologia.Tag = (_filtroAtivo == "Tecnologia") ? "ativo" : null;
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            string busca = TxtBusca.Text.ToLower();
            FiltrarCards(busca);
        }

        private void FiltrarCards(string busca)
        {
            var cards = new[] { CardCurso1, CardCurso2, CardCurso3, CardCurso4, CardCurso5 };
            var nomes = new[] {
                "algoritmos e estrutura de dados",
                "calculo i - limites e derivadas",
                "engenharia de software",
                "banco de dados relacional",
                "programacao orientada a objetos em c#"
            };
            var categorias = new[] { "Exatas", "Exatas", "Humanas", "Tecnologia", "Tecnologia" };

            for (int i = 0; i < cards.Length; i++)
            {
                bool matchBusca = string.IsNullOrEmpty(busca) || nomes[i].Contains(busca);
                bool matchFiltro = _filtroAtivo == "Todos" || categorias[i] == _filtroAtivo;
                cards[i].Visibility = (matchBusca && matchFiltro) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (_aluno == null) return;

            var btn = sender as Button;
            if (btn == null) return;

            var card = EncontrarCard(btn);
            if (card == null) return;

            int cursoId = ObterCursoId(card.Name);
            if (cursoId <= 0)
            {
                MessageBox.Show("Curso nao identificado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new LearnixDbContext();
            bool jaMatriculado = ctx.Matriculas.Any(m => m.AlunoId == _aluno.Id && m.CursoId == cursoId);
            if (jaMatriculado)
            {
                MessageBox.Show("Voce ja esta matriculado neste curso!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var service = new MatriculaService(ctx);
            var matricula = service.CriarMatricula(_aluno.Id, cursoId);
            if (matricula != null)
            {
                MessageBox.Show("Matricula realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                if (Window.GetWindow(this) is MainWindow mw)
                    mw.MostrarMeusCursos(_aluno.Nome ?? "Aluno");
            }
            else
            {
                MessageBox.Show("Erro ao realizar matricula.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border? EncontrarCard(DependencyObject filho)
        {
            DependencyObject? atual = filho;
            while (atual != null)
            {
                if (atual is Border b && (b.Name?.StartsWith("CardCurso") == true))
                    return b;
                atual = VisualTreeHelper.GetParent(atual);
            }
            return null;
        }

        private int ObterCursoId(string nomeCard)
        {
            using var ctx = new LearnixDbContext();
            var nomes = new System.Collections.Generic.Dictionary<string, string>
            {
                { "CardCurso1", "Algoritmos e Estrutura de Dados" },
                { "CardCurso2", "Calculo I - Limites e Derivadas" },
                { "CardCurso3", "Engenharia de Software" },
                { "CardCurso4", "Banco de Dados Relacional" },
                { "CardCurso5", "Programacao Orientada a Objetos em C#" }
            };

            if (!nomes.TryGetValue(nomeCard, out var nome)) return 0;
            return ctx.Cursos.Where(c => c.Titulo == nome).Select(c => c.Id).FirstOrDefault();
        }
    }
}
