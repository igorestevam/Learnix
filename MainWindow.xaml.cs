using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Learnix.data;
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

        /// <summary>
        /// Recarrega o Aluno do banco com HistoricoMatriculas e relacionamentos completos.
        /// Centralizado aqui para evitar repeticao em cada tela.
        /// </summary>
        private Aluno? RecarregarAlunoCompleto(int alunoId)
        {
            using var ctx = new LearnixDbContext();
            return ctx.Alunos
                .Include(a => a.Perfil)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Instrutor)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Categoria)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c.Modulos)
                            .ThenInclude(mod => mod.Aulas)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Progresso)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Certificado)
                .Include(a => a.HistoricoMatriculas)
                    .ThenInclude(m => m.Avaliacoes)
                .FirstOrDefault(a => a.Id == alunoId);
        }

        // ── Telas de autenticacao ────────────────────────────────────────────

        private void MostrarLogin()
        {
            var tela = new TelaLogin();
            tela.SolicitarCadastro += (s, e) => MostrarCadastro();
            tela.SolicitarRecuperacaoSenha += (s, e) => MostrarEsqueceuSenha();
            tela.SolicitarHome += (s, e, usuario) =>
            {
                // Se for aluno, recarrega com todos os relacionamentos
                if (usuario is Aluno alunoLogin)
                {
                    var alunoCompleto = RecarregarAlunoCompleto(alunoLogin.Id);
                    _usuarioLogado = alunoCompleto ?? usuario;
                }
                else
                {
                    _usuarioLogado = usuario;
                }
                MostrarHome(_usuarioLogado);
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

        public void MostrarHome(Usuario? usuario)
        {
            AjustarJanela(1280, 720);
            string nome = usuario?.Nome ?? "Aluno";
            var tela = new TelaHome();

            // Se for Aluno, passa o objeto completo para popular os cards com dados reais
            if (usuario is Aluno aluno)
                tela.DefinirAluno(aluno);
            else
                tela.DefinirAluno(nome);

            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarNotas(Matricula matricula)
        {
            AjustarJanela(1280, 720);
            var tela = new TelaNotas();
            tela.DefinirMatricula(matricula);
            ConectarSidebar(tela.Sidebar, matricula?.Aluno?.Nome ?? _usuarioLogado?.Nome ?? "Aluno");
            conteudoPrincipal.Content = tela;
        }

        public void MostrarMeusCursos(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            if (_usuarioLogado is Aluno aluno)
            {
                // Recarrega para garantir dados atualizados
                var alunoAtualizado = RecarregarAlunoCompleto(aluno.Id) ?? aluno;
                _usuarioLogado = alunoAtualizado;

                var tela = new TelaMeusCursos();
                tela.DefinirAluno(alunoAtualizado);
                ConectarSidebar(tela.Sidebar, alunoAtualizado.Nome);
                conteudoPrincipal.Content = tela;
            }
        }

        public void MostrarMenu(string nomeAluno = "Aluno")
        {
            AjustarJanela(1280, 720);
            string nome = _usuarioLogado?.Nome ?? nomeAluno;
            var tela = new TelaMenu();
            tela.Sidebar.DefinirAluno(nome);

            // Passa o usuario para que a TelaMenu possa criar matriculas em nome dele
            if (_usuarioLogado is Aluno aluno)
            {
                tela.DefinirAluno(aluno);
            }

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
                var alunoAtualizado = RecarregarAlunoCompleto(aluno.Id) ?? aluno;
                _usuarioLogado = alunoAtualizado;

                var tela = new TelaCertificados();
                tela.DefinirAluno(alunoAtualizado);
                if (tela.SidebarNav != null)
                    ConectarSidebar(tela.SidebarNav, alunoAtualizado.Nome);
                conteudoPrincipal.Content = tela;
            }
        }

        public void MostrarAulas(Matricula matricula)
        {
            AjustarJanela(1280, 720);
            // Nome vem do _usuarioLogado (fonte confiavel) em vez de matricula.Aluno
            string nome = _usuarioLogado?.Nome ?? "Aluno";
            var tela = new TelaAulas();
            tela.DefinirMatricula(matricula, nome);
            ConectarSidebar(tela.Sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        public void MostrarTela(UserControl tela, string nomeAluno = "")
        {
            AjustarJanela(1280, 720);
            // Se nomeAluno nao foi passado, usa o do usuario logado
            string nome = string.IsNullOrWhiteSpace(nomeAluno)
                ? (_usuarioLogado?.Nome ?? "Aluno")
                : nomeAluno;

            var sidebarProp = tela.GetType().GetProperty("Sidebar");
            if (sidebarProp?.GetValue(tela) is SidebarControl sidebar)
                ConectarSidebar(sidebar, nome);
            conteudoPrincipal.Content = tela;
        }

        // ── Sidebar centralizada ─────────────────────────────────────────────

        private void ConectarSidebar(SidebarControl sidebar, string nomeAluno)
        {
            sidebar.SolicitarMenu += (s, e) => MostrarMenu(nomeAluno);
            sidebar.SolicitarMeusCursos += (s, e) => MostrarMeusCursos(nomeAluno);
            sidebar.SolicitarNotas += (s, e) =>
            {
                if (_usuarioLogado is Aluno aluno)
                {
                    // Recarrega para garantir avaliacoes atualizadas
                    var alunoAtualizado = RecarregarAlunoCompleto(aluno.Id) ?? aluno;
                    _usuarioLogado = alunoAtualizado;

                    var primeira = alunoAtualizado.HistoricoMatriculas?.FirstOrDefault();
                    if (primeira != null) MostrarNotas(primeira);
                }
            };
            sidebar.SolicitarCertificados += (s, e) => MostrarCertificados(nomeAluno);
            sidebar.SolicitarPerfil += (s, e) => MostrarPerfil(nomeAluno);
            sidebar.SolicitarSair += (s, e) =>
            {
                _usuarioLogado = null;
                AjustarJanela(500, 480, false);
                MostrarLogin();
            };
        }

        private void AjustarJanela(double width, double height, bool comMinimo = true)
        {
            this.Width = width;
            this.Height = height;
            if (comMinimo)
            {
                this.MinWidth = width;
                this.MinHeight = height;
            }
            this.WindowState = WindowState.Normal;
        }
    }
}