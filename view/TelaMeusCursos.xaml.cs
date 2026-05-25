using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private string _nomeAluno = string.Empty;
        private List<Matricula> _matriculas = new();
        private Aluno? _alunoLogado;

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _alunoLogado = aluno;
            _nomeAluno = aluno.Nome;
            _matriculas = aluno.HistoricoMatriculas?.ToList() ?? new List<Matricula>();
            Sidebar?.DefinirAluno(aluno.Nome);
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null) return;

            // Tenta interpretar Tag como ID de matricula ou nome do curso
            string tag = btn.Tag.ToString() ?? string.Empty;
            Matricula? matricula = null;

            if (int.TryParse(tag, out int matriculaId))
            {
                matricula = _matriculas.Find(m => m.Id == matriculaId);
            }

            if (matricula == null)
            {
                // Fallback: busca por nome do curso
                matricula = _matriculas.Find(m => m.Curso != null && m.Curso.Titulo.Contains(tag, StringComparison.OrdinalIgnoreCase));
            }

            if (matricula == null)
            {
                MessageBox.Show("Voce ainda nao esta matriculado nesse curso.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarAulas(matricula);
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null) return;

            string tag = btn.Tag.ToString() ?? string.Empty;
            Matricula? matricula = null;

            if (int.TryParse(tag, out int matriculaId))
            {
                matricula = _matriculas.Find(m => m.Id == matriculaId);
            }
            if (matricula == null)
            {
                matricula = _matriculas.Find(m => m.Curso != null && m.Curso.Titulo.Contains(tag, StringComparison.OrdinalIgnoreCase));
            }

            if (matricula == null)
            {
                MessageBox.Show("Voce ainda nao esta matriculado nesse curso.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using var ctx = new LearnixDbContext();
                var matriculaCompleta = ctx.Matriculas
                    .Include(m => m.Curso)
                    .ThenInclude(c => c.Modulos)
                    .ThenInclude(mo => mo.Aulas)
                    .FirstOrDefault(m => m.Id == matricula.Id);

                if (matriculaCompleta?.Curso == null)
                {
                    MessageBox.Show("Matricula nao encontrada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var progSvc = new ProgressoService(ctx);
                int concluidas = 0;
                foreach (var modulo in matriculaCompleta.Curso.Modulos)
                {
                    foreach (var aula in modulo.Aulas)
                    {
                        progSvc.RegistrarConclusaoAula(matriculaCompleta.Id, aula.Id);
                        concluidas++;
                    }
                }

                btn.IsEnabled = false;
                btn.Content = "Concluido";
                MessageBox.Show($"Parabens! Voce concluiu o curso. {concluidas} aula(s) marcadas como concluidas.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao concluir curso: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
