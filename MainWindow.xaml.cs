using System.Windows;
using System.Windows.Controls;
using Learnix.model;

namespace Learnix
{
    public partial class MainWindow : Window
    {
        // Armazena o usuario logado para propagar entre telas
        private Usuario? _usuarioLogado;

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
            tela.SolicitarHome += (s, e, usuario) =>
            {
                _usuarioLogado = usuario;
                MostrarHome(usuario);
            };
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

        public void MostrarHome(Usuario usuario)
        {
            AjustarJanela(1280, 720);
            string nome = usuario?.Nome ?? "Aluno";
            var tela = new TelaHome();
            tela.DefinirAluno(nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarNotas(Matricula matricula)
        {
            AjustarJanela(1280, 720);
            var tela = new TelaNotas();
            tela.DefinirMatricula(matricula);
            ConectarSidebar(tela.Sidebar, matricula?.Aluno?.Nome ?? "Aluno");
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMeusCursos(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            if (_usuarioLogado is Aluno aluno)
            {
                var tela = new TelaMeusCursos();
                tela.DefinirAluno(aluno);
                ConectarSidebar(tela.Sidebar, aluno.Nome);
                conteudoPrincipal.Content = tela;
            }
        }

        public void MostrarMenu(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            string nome = _usuarioLogado?.Nome ?? nomeAluno;
            var tela = new TelaMenu();
            tela.Sidebar.DefinirAluno(nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarPerfil(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            if (_usuarioLogado is Aluno aluno)
            {
                var tela = new TelaPerfil();
                tela.DefinirAluno(aluno);
                ConectarSidebar(tela.Sidebar, aluno.Nome);
                conteudoPrincipal.Content = tela;
            }
        }

        public void MostrarCertificados(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            if (_usuarioLogado is Aluno aluno)
            {
                var tela = new TelaCertificados();
                tela.DefinirAluno(aluno);
                if (tela.SidebarNav != null)
                    ConectarSidebar(tela.SidebarNav, aluno.Nome);
                conteudoPrincipal.Content = tela;
            }
        }

        public void MostrarAulas(Matricula matricula)
        {
            AjustarJanela(1280, 720);
            string nome = _usuarioLogado?.Nome ?? "Aluno";
            var tela = new TelaAulas();
            tela.DefinirMatricula(matricula, nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        // Método genérico para telas que já vêm configuradas (ex: TelaPlayer)
        public void MostrarTela(UserControl tela, string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);

            var sidebarProp = tela.GetType().GetProperty("Sidebar");
            if (sidebarProp?.GetValue(tela) is SidebarControl sidebar)
                ConectarSidebar(sidebar, nomeAluno);

            conteudoPrincipal.Content = tela;
        }

        // ── Sidebar centralizada (Clean Code / DRY) ──────────────────────────

        private void ConectarSidebar(SidebarControl sidebar, string nomeAluno)
        {
            sidebar.SolicitarMenu       += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarNotas      += (s, e) => MostrarNotas(_usuarioLogado is Aluno a && a.HistoricoMatriculas?.Count > 0
                                                        ? a.HistoricoMatriculas[0] : null);
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursos(nomeAluno);
            sidebar.SolicitarCertificados += (s, e) => MostrarCertificados(nomeAluno);
            sidebar.SolicitarPerfil     += (s, e) => MostrarPerfil(nomeAluno);
            sidebar.SolicitarSair       += (s, e) =>
            {
                var r = MessageBox.Show("Deseja sair da sua conta?", "Learnix",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    _usuarioLogado = null;
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
