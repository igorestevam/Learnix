using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private string _nomeAluno = "Aluno";
        private List<Matricula> _matriculas = new();

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _nomeAluno = aluno.Nome;
            _matriculas = aluno.HistoricoMatriculas ?? new List<Matricula>();
            Sidebar.DefinirAluno(aluno.Nome);
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string dados)
            {
                var p = dados.Split('|');
                var main = Application.Current.MainWindow as MainWindow;

                // Tenta encontrar a matricula correspondente pelo nome do curso
                string nomeCurso = p.Length > 0 ? p[0] : "";
                var matricula = _matriculas.Find(m => m.Curso?.Titulo == nomeCurso);

                if (matricula != null)
                {
                    main?.MostrarAulas(matricula);
                }
                else
                {
                    // Fallback: navega com os dados da string (cards estáticos)
                    MessageBox.Show("Curso não encontrado no banco de dados.",
                        "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string dados)
            {
                var p = dados.Split('|');
                string nomeCurso    = p.Length > 0 ? p[0] : "";
                string professor    = p.Length > 1 ? p[1] : "";
                string cargaHoraria = p.Length > 3 ? p[3] : "";

                // Tenta registrar via ProgressoService se houver matricula no banco
                var matricula = _matriculas.Find(m => m.Curso?.Titulo == nomeCurso);
                if (matricula != null)
                {
                    var progressoService = new ProgressoService(new LearnixDbContext());
                    foreach (var modulo in matricula.Curso?.Modulos ?? new List<Modulo>())
                        foreach (var aula in modulo.Aulas)
                            progressoService.RegistrarConclusaoAula(matricula.Id, aula.Id);
                }

                btn.IsEnabled = false;
                btn.Content = "\u2714 Concluído";

                string msg = "Parabéns! Você concluiu o curso \"" + nomeCurso + "\"!\n\n"
                    + "Seu certificado já está disponível em \"Meus Certificados\".\n\n"
                    + "Deseja ver o certificado agora?";

                var resultado = MessageBox.Show(msg,
                    "Learnix", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (resultado == MessageBoxResult.Yes)
                {
                    var main = Application.Current.MainWindow as MainWindow;
                    main?.MostrarCertificados(_nomeAluno);
                }
            }
        }
    }
}
