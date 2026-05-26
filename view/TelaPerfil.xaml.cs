using System;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix.view
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
            CarregarDados();
        }

        private void CarregarDados()
        {
            if (_aluno == null) return;

            TxtIniciais.Text = ObterIniciais(_aluno.Nome ?? "");
            TxtNomePerfil.Text = _aluno.Nome ?? "";
            TxtEmailPerfil.Text = _aluno.Email ?? "";

            TxtEditNome.Text = _aluno.Nome ?? "";
            TxtEditEmail.Text = _aluno.Email ?? "";
            TxtEditTelefone.Text = "";
            TxtEditNascimento.Text = "";

            TxtMatricula.Text = _aluno.MatriculaAcademica ?? "";
            TxtEstilo.Text = _aluno.Perfil?.EstiloPredominante ?? "Nao definido";
            TxtRitmo.Text = _aluno.Perfil?.RitmoSugerido ?? "Nao definido";
        }

        private string ObterIniciais(string nome)
        {
            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
            return (partes[0][0].ToString() + partes[^1][0].ToString()).ToUpper();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            BtnEditar.Visibility = Visibility.Collapsed;
            BtnSalvar.Visibility = Visibility.Visible;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (_aluno == null) return;

            _aluno.Nome = TxtEditNome.Text;
            _aluno.Email = TxtEditEmail.Text;

            using var ctx = new LearnixDbContext();
            ctx.Alunos.Update(_aluno);
            ctx.SaveChanges();

            CarregarDados();
            BtnEditar.Visibility = Visibility.Visible;
            BtnSalvar.Visibility = Visibility.Collapsed;
        }
    }
}
