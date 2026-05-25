using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private Aluno? _aluno;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            Sidebar?.DefinirAluno(aluno.Nome);

            TxtNomePerfil.Text = aluno.Nome;
            TxtEmailPerfil.Text = aluno.Email ?? string.Empty;

            // Iniciais
            string iniciais;
            var partes = aluno.Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 2)
                iniciais = (partes[0][0].ToString() + partes[1][0].ToString()).ToUpper();
            else if (partes.Length == 1 && partes[0].Length >= 1)
                iniciais = partes[0][0].ToString().ToUpper();
            else
                iniciais = "?";
            TxtIniciais.Text = iniciais;

            // Pre-popula campos de edicao
            TxtEditNome.Text = aluno.Nome;
            TxtEditEmail.Text = aluno.Email ?? string.Empty;
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            BtnEditar.Visibility = Visibility.Collapsed;
            BtnSalvar.Visibility = Visibility.Visible;
            TxtEditNome.IsReadOnly = false;
            TxtEditEmail.IsReadOnly = false;
            TxtEditTelefone.IsReadOnly = false;
            TxtEditNascimento.IsReadOnly = false;
            var brushAtivo = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            TxtEditNome.Background = brushAtivo;
            TxtEditEmail.Background = brushAtivo;
            TxtEditTelefone.Background = brushAtivo;
            TxtEditNascimento.Background = brushAtivo;
            TxtEditNome.Focus();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (_aluno == null)
            {
                MessageBox.Show("Nenhum aluno carregado.", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtEditNome.Text) || string.IsNullOrWhiteSpace(TxtEditEmail.Text))
            {
                MessageBox.Show("Nome e email sao obrigatorios.", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var ctx = new LearnixDbContext();
                var aluno = ctx.Alunos.FirstOrDefault(a => a.Id == _aluno.Id);
                if (aluno == null)
                {
                    MessageBox.Show("Aluno nao encontrado no banco.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                aluno.Nome = TxtEditNome.Text.Trim();
                aluno.Email = TxtEditEmail.Text.Trim();
                ctx.SaveChanges();

                _aluno.Nome = aluno.Nome;
                _aluno.Email = aluno.Email;
                TxtNomePerfil.Text = aluno.Nome;
                TxtEmailPerfil.Text = aluno.Email ?? string.Empty;

                BtnSalvar.Visibility = Visibility.Collapsed;
                BtnEditar.Visibility = Visibility.Visible;
                TxtEditNome.IsReadOnly = true;
                TxtEditEmail.IsReadOnly = true;
                TxtEditTelefone.IsReadOnly = true;
                TxtEditNascimento.IsReadOnly = true;
                var brushReadonly = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                TxtEditNome.Background = brushReadonly;
                TxtEditEmail.Background = brushReadonly;
                TxtEditTelefone.Background = brushReadonly;
                TxtEditNascimento.Background = brushReadonly;

                MessageBox.Show("Perfil atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
