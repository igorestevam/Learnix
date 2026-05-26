using System.Windows;
using System.Windows.Controls;
using Learnix.data;
using Learnix.model;
using Microsoft.EntityFrameworkCore;

namespace Learnix
{
    public partial class MainWindow : Window
    {
        private Usuario? _usuarioLogado;

        public MainWindow()
        {
            InitializeComponent();
            MostrarLogin();
        }

        // ── Recarrega o aluno do banco ───────────────────────────────────────

        private Aluno? AlunoAtual()
        {
            if (_usuarioLogado is not Aluno aluno) return null;
            using var db = new LearnixDbContext();
            return db.Alunos
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                .FirstOrDefault(a => a.Id == aluno.Id);
        }

        private Instrutor? InstrutorAtual()
        {
            if (_usuarioLogado is not Instrutor instrutor) return null;
            using var db = new LearnixDbContext();
            return db.Instrutores
                .Include(i => i.Cursos)
                .FirstOrDefault(i => i.Id == instrutor.Id);
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
                if (usuario is Instrutor instrutor)
                    MostrarHomeInstrutor(instrutor);
                else
                    MostrarHome();
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

        // ── Telas do Aluno ───────────────────────────────────────────────────

        public void MostrarHome(Usuario? usuario = null)
        {
            if (usuario != null) _usuarioLogado = usuario;
            AjustarJanela(1280, 720);

            var aluno = AlunoAtual();
            var tela = new TelaHome();

            if (aluno != null)
                tela.DefinirAluno(aluno);
            else
                tela.DefinirAluno(_usuarioLogado?.Nome ?? "Aluno");

            ConectarSidebar(tela.Sidebar, aluno?.Nome ?? _usuarioLogado?.Nome ?? "Aluno");
            conteudoPrincipal.Content = tela;
        }

        public void MostrarNotas()
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            if (aluno == null) return;

            var tela = new TelaNotas();
            tela.DefinirAluno(aluno);
            ConectarSidebar(tela.Sidebar, aluno.Nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarNotas(Matricula? matricula)
        {
            MostrarNotas();
        }

        public void MostrarMeusCursos(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            if (aluno == null) return;

            var tela = new TelaMeusCursos();
            tela.DefinirAluno(aluno);
            ConectarSidebar(tela.Sidebar, aluno.Nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMenu(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            var tela = new TelaMenu();

            if (aluno != null)
                tela.DefinirAluno(aluno);

            string nome = aluno?.Nome ?? _usuarioLogado?.Nome ?? nomeAluno;
            tela.Sidebar.DefinirAluno(nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarPerfil(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            if (aluno == null) return;

            var tela = new TelaPerfil();
            tela.DefinirAluno(aluno);
            ConectarSidebar(tela.Sidebar, aluno.Nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarCertificados(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            if (aluno == null) return;

            var tela = new TelaCertificados();
            tela.DefinirAluno(aluno);
            if (tela.SidebarNav != null)
                ConectarSidebar(tela.SidebarNav, aluno.Nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarAulas(Matricula matricula)
        {
            AjustarJanela(1280, 720);
            var aluno = AlunoAtual();
            string nome = aluno?.Nome ?? _usuarioLogado?.Nome ?? "Aluno";

            var tela = new TelaAulas();
            tela.DefinirMatricula(matricula, nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarTela(UserControl tela, string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            var sidebarProp = tela.GetType().GetProperty("Sidebar");
            if (sidebarProp?.GetValue(tela) is SidebarControl sidebar)
                ConectarSidebar(sidebar, nomeAluno);
            conteudoPrincipal.Content = tela;
        }

        // ── Telas do Instrutor ───────────────────────────────────────────────

        public void MostrarHomeInstrutor(Instrutor? instrutor = null)
        {
            AjustarJanela(1280, 720);
            var inst = instrutor ?? InstrutorAtual();
            if (inst == null) return;

            var tela = new TelaHomeInstrutor();
            tela.DefinirInstrutor(inst);
            ConectarSidebarInstrutor(tela.Sidebar, inst);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarLancarNotas(Instrutor? instrutor = null)
        {
            AjustarJanela(1280, 720);
            var inst = instrutor ?? InstrutorAtual();
            if (inst == null) return;

            var tela = new TelaLancarNotas();
            tela.DefinirInstrutor(inst);
            ConectarSidebarInstrutor(tela.Sidebar, inst);
            conteudoPrincipal.Content = tela;
        }

        // ── Sidebars ─────────────────────────────────────────────────────────

        private void ConectarSidebar(SidebarControl sidebar, string nomeAluno)
        {
            sidebar.SolicitarHome += (s, e) => MostrarHome();
            sidebar.SolicitarBuscarCursos += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarMenu += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarNotas += (s, e) => MostrarNotas();
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursos(nomeAluno);
            sidebar.SolicitarCertificados += (s, e) => MostrarCertificados(nomeAluno);
            sidebar.SolicitarPerfil += (s, e) => MostrarPerfil(nomeAluno);
            sidebar.SolicitarSair += (s, e) =>
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

        public void MostrarEscolherCurso(Instrutor? instrutor = null)
        {
            AjustarJanela(1280, 720);
            var inst = instrutor ?? InstrutorAtual();
            if (inst == null) return;

            var tela = new TelaEscolherCurso();
            tela.DefinirInstrutor(inst);
            ConectarSidebarInstrutor(tela.Sidebar, inst);
            conteudoPrincipal.Content = tela;
        }

        private void ConectarSidebarInstrutor(SidebarInstrutor sidebar, Instrutor instrutor)
        {
            sidebar.SolicitarHome += (s, e) => MostrarHomeInstrutor(instrutor);
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursosInstrutor(instrutor);
            sidebar.SolicitarLancarNotas += (s, e) => MostrarLancarNotas(instrutor);
            sidebar.SolicitarEscolherCurso += (s, e) => MostrarEscolherCurso(instrutor);
            sidebar.SolicitarSair += (s, e) =>
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

        public void MostrarMeusCursosInstrutor(Instrutor? instrutor = null)
        {
            AjustarJanela(1280, 720);
            var inst = instrutor ?? InstrutorAtual();
            if (inst == null) return;

            var tela = new TelaMeusCursosInstrutor();
            tela.DefinirInstrutor(inst);
            ConectarSidebarInstrutor(tela.Sidebar, inst);
            conteudoPrincipal.Content = tela;
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