using System.Windows;
using System.Windows.Controls;

namespace Learnix
{
    public partial class TelaMeusCursos : UserControl
    {
        private string _nomeAluno = "Aluno";

        public TelaMeusCursos()
        {
            InitializeComponent();
        }

        public void DefinirAluno(string nome)
        {
            _nomeAluno = nome;
            Sidebar.DefinirAluno(nome);
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string dados)
            {
                var p    = dados.Split('|');
                var main = Application.Current.MainWindow as MainWindow;
                main?.MostrarAulas(
                    nomeAluno:    _nomeAluno,
                    nomeCurso:    p.Length > 0 ? p[0] : "",
                    professor:    p.Length > 1 ? p[1] : "",
                    categoria:    p.Length > 2 ? p[2] : "",
                    cargaHoraria: p.Length > 3 ? p[3] : "",
                    descricao:    p.Length > 4 ? p[4] : "",
                    progresso:    p.Length > 5 ? p[5] : "0%"
                );
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

                // Registra o certificado na sessão
                TelaCertificados.AdicionarCertificado(_nomeAluno, nomeCurso, professor, cargaHoraria);

                // Desabilita o botão para não emitir duas vezes
                btn.IsEnabled = false;
                btn.Content   = "\u2714 Concluído";

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
