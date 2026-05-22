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

        // ── Telas de autenticação (padrão já existente) ──────────────────────

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

        // ── Telas principais (com sidebar) ───────────────────────────────────

        public void MostrarHome(string nomeAluno = "Aluno")
        {
            var tela = new TelaHome();
            tela.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;

            // Janela maior para acomodar sidebar + conteúdo
            Width = 1280;
            Height = 720;
            MinWidth = 900;
            MinHeight = 600;
        }

        public void MostrarNotas(string nomeAluno = "Aluno")
        {
            var tela = new TelaNotas();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMeusCursos(string nomeAluno = "Aluno")
        {
            var tela = new TelaMeusCursos();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMenu(string nomeAluno = "Aluno")
        {
            var tela = new TelaMenu();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarPerfil(string nomeAluno = "Aluno")
        {
            var tela = new TelaPerfil();
            tela.Sidebar.DefinirAluno(nomeAluno);
            ConectarSidebar(tela.Sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        // ── Conecta sidebar de qualquer tela principal ────────────────────────
        // Centraliza a lógica para não repetir em cada tela (Clean Code / DRY)

        private void ConectarSidebar(SidebarControl sidebar, string nomeAluno)
        {
            sidebar.SolicitarMenu += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarNotas += (s, e) => MostrarNotas(nomeAluno);
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursos(nomeAluno);
            sidebar.SolicitarPerfil += (s, e) => MostrarPerfil(nomeAluno);
            sidebar.SolicitarSair += (s, e) =>
            {
                var r = MessageBox.Show("Deseja sair da sua conta?", "Learnix",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    Width = 500;
                    Height = 480;
                    MinWidth = 0;
                    MinHeight = 0;
                    MostrarLogin();
                }
            };
        }
    }
}
