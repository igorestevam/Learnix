using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;

namespace Learnix.view
{
    public partial class TelaMeusCursos : UserControl
    {
        private Aluno? _aluno;

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is not string tag) return;

            string nomeCurso = tag.Split('|')[0];
            IrParaCurso(nomeCurso);
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is not string tag) return;

            string nomeCurso = tag.Split('|')[0];
            IrParaCurso(nomeCurso);
        }

        private void IrParaCurso(string nomeCurso)
        {
            if (_aluno == null) return;

            using var ctx = new LearnixDbContext();
            var matricula = ctx.Matriculas
                .Include(m => m.Curso)
                .FirstOrDefault(m => m.AlunoId == _aluno.Id
                    && m.Curso != null
                    && m.Curso.Titulo == nomeCurso);

            if (matricula == null)
            {
                MessageBox.Show("Matricula nao encontrada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Window.GetWindow(this) is MainWindow mw)
                mw.MostrarAulas(matricula);
        }
    }
}
