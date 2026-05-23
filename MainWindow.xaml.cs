using System.Windows;
using System.Windows.Controls;

namespace Learnix
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MostrarLogin();
        }

        // ── Telas de autenticação ────────────────────────────────────────────

        private void MostrarLogin()
        {
            var tela = new TelaLogin();
            tela.SolicitarCadastro += (s, e) => MostrarCadastro();
            tela.SolicitarRecuperacaoSenha += (s, e) => MostrarEsqueceuSenha();
            tela.SolicitarHome += (s, e, nome) => MostrarHome(nome);
            conteudoPrincipal.Content = tela;
        }

        private void MostrarCadastro()
        {
            var tela = new TelaCadastro();
            tela.SolicitarLogin += (s, e) => MostrarLogin();
            conteudoPrincipal.Content = tela;
        }

        private void MostrarEsqueceuSenha()
        {
            var tela = new TelaEsqueceuSenha();
            tela.SolicitarLogin += (s, e) => MostrarLogin();
            conteudoPrincipal.Content = tela;
        }

        // ── Telas principais ─────────────────────────────────────────────────

        public void MostrarHome(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaHome();
            tela.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarNotas(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaNotas();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMeusCursos(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaMeusCursos();
            tela.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMenu(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaMenu();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarPerfil(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaPerfil();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarCertificados(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaCertificados();
            tela.DefinirAluno(nomeAluno);
            if (tela.SidebarNav != null)
                ConectarSidebar(tela.SidebarNav, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarAulas(string nomeAluno = "Aluno",
            string nomeCurso = "", string professor = "",
            string categoria = "", string cargaHoraria = "",
            string descricao = "", string progresso = "0%")
        {
            AjustarJanela(1280, 720);
            var tela = new TelaAulas();
            tela.DefinirAluno(nomeAluno);
            tela.DefinirCurso(nomeCurso, professor, categoria, cargaHoraria, descricao, progresso);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        // Método genérico para telas que já vêm configuradas (ex: TelaPlayer, TelaAulas)
        public void MostrarTela(UserControl tela, string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);

            // Tenta conectar a sidebar se a tela tiver uma
            var sidebarProp = tela.GetType().GetProperty("Sidebar");
            if (sidebarProp?.GetValue(tela) is SidebarControl sidebar)
                ConectarSidebar(sidebar, nomeAluno);

            conteudoPrincipal.Content = tela;
        }

        // ── Sidebar centralizada (Clean Code / DRY) ──────────────────────────

        private void ConectarSidebar(SidebarControl sidebar, string nomeAluno)
        {
            sidebar.SolicitarMenu += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarNotas += (s, e) => MostrarNotas(nomeAluno);
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursos(nomeAluno);
            sidebar.SolicitarCertificados += (s, e) => MostrarCertificados(nomeAluno);
            sidebar.SolicitarPerfil += (s, e) => MostrarPerfil(nomeAluno);
            sidebar.SolicitarSair += (s, e) =>
            {
                var r = MessageBox.Show("Deseja sair da sua conta?", "Learnix",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    AjustarJanela(500, 480, false);
                    MostrarLogin();
                }
            };
        }

        private void AjustarJanela(double width, double height, bool comMinimo = true)
        {
            Width = width;
            Height = height;
            if (comMinimo) { MinWidth = 900; MinHeight = 600; }
            else { MinWidth = 0; MinHeight = 0; }
        }
    }
}
