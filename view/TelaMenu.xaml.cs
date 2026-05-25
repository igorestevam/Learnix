using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        private string _categoriaAtiva = "Todos";
        private Aluno? _alunoLogado;

        // Mapeia o nome do Border do card para o titulo do curso seedado no banco.
        // Permite localizar o curso correto ao clicar em Matricular sem depender de Tag no botao.
        private static readonly System.Collections.Generic.Dictionary<string, string> MapCardCursoParaTitulo = new()
        {
            { "CardCurso1", "Algoritmos e Estrutura de Dados" },
            { "CardCurso2", "Calculo I - Limites e Derivadas" },
            { "CardCurso3", "Engenharia de Software" },
            { "CardCurso4", "Banco de Dados Relacional" },
            { "CardCurso5", "Programacao Orientada a Objetos em C#" }
        };

        public TelaMenu()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _alunoLogado = aluno;
            Sidebar?.DefinirAluno(aluno.Nome);
        }

        private void TxtBusca_GotFocus(object sender, RoutedEventArgs e)
        {
            // Placeholder opcional - tela esta 100% pronta sem PlaceholderBusca
        }

        private void TxtBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            // Placeholder opcional
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void FiltroTodos_Click(object sender, RoutedEventArgs e) => AtivarCategoria("Todos");
        private void FiltroExatas_Click(object sender, RoutedEventArgs e) => AtivarCategoria("Exatas");
        private void FiltroHumanas_Click(object sender, RoutedEventArgs e) => AtivarCategoria("Humanas");
        private void FiltroTecnologia_Click(object sender, RoutedEventArgs e) => AtivarCategoria("Tecnologia");

        private void AtivarCategoria(string categoria)
        {
            _categoriaAtiva = categoria;
            var brushAtivo = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7B5BA6"));
            var brushInativo = new SolidColorBrush(Colors.Transparent);

            BtnTodos.Background = (categoria == "Todos") ? brushAtivo : brushInativo;
            BtnExatas.Background = (categoria == "Exatas") ? brushAtivo : brushInativo;
            BtnHumanas.Background = (categoria == "Humanas") ? brushAtivo : brushInativo;
            BtnTecnologia.Background = (categoria == "Tecnologia") ? brushAtivo : brushInativo;

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            string busca = (TxtBusca?.Text ?? string.Empty).Trim().ToLower();

            if (PainelCursos == null) return;
            foreach (var child in PainelCursos.Children)
            {
                if (child is FrameworkElement fe)
                {
                    string tag = fe.Tag?.ToString() ?? string.Empty;
                    string titulo = EncontrarTitulo(fe);
                    bool matchCategoria = _categoriaAtiva == "Todos" || tag.Contains(_categoriaAtiva, StringComparison.OrdinalIgnoreCase);
                    bool matchBusca = string.IsNullOrEmpty(busca) || titulo.ToLower().Contains(busca);
                    fe.Visibility = (matchCategoria && matchBusca) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private string EncontrarTitulo(DependencyObject card)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(card); i++)
            {
                var c = VisualTreeHelper.GetChild(card, i);
                if (c is TextBlock tb && tb.FontWeight == FontWeights.Bold && !string.IsNullOrWhiteSpace(tb.Text))
                    return tb.Text;
                string sub = EncontrarTitulo(c);
                if (!string.IsNullOrEmpty(sub)) return sub;
            }
            return string.Empty;
        }

        private void BtnMatricular_Click(object sender, RoutedEventArgs e)
        {
            if (_alunoLogado == null)
            {
                MessageBox.Show("Voce precisa estar logado para se matricular.", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Sobe a arvore visual ate achar o Border do card (CardCursoN)
            DependencyObject? atual = sender as DependencyObject;
            string nomeCard = string.Empty;
            while (atual != null)
            {
                if (atual is FrameworkElement fe && !string.IsNullOrEmpty(fe.Name) && fe.Name.StartsWith("CardCurso"))
                {
                    nomeCard = fe.Name;
                    break;
                }
                atual = VisualTreeHelper.GetParent(atual);
            }

            if (string.IsNullOrEmpty(nomeCard) || !MapCardCursoParaTitulo.TryGetValue(nomeCard, out string? titulo))
            {
                MessageBox.Show("Nao foi possivel identificar o curso.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var ctx = new LearnixDbContext();
                var curso = ctx.Cursos.FirstOrDefault(c => c.Titulo == titulo);
                if (curso == null)
                {
                    MessageBox.Show("Curso ainda nao disponivel para matricula.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var matriculaSvc = new MatriculaService(ctx);
                var matricula = matriculaSvc.CriarMatricula(_alunoLogado.Id, curso.Id);
                if (matricula != null)
                {
                    MessageBox.Show($"Matricula em \"{curso.Titulo}\" realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Voce ja esta matriculado neste curso.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao realizar matricula: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
