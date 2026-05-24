# Learnix — Backup das Alterações

Backup completo do antes e depois de cada arquivo alterado conforme o plano de correções MVC.
Gerado em 24/05/2026.

---

## 1. `view/TelaLogin.xaml.cs`

**Commit:** `fix: TelaLogin - integrar AuthService e propagar objeto Usuario`

### ANTES

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        public event RoutedEventHandler? SolicitarCadastro;
        public event RoutedEventHandler? SolicitarRecuperacaoSenha;

        // Novo: passa o nome do aluno após login bem-sucedido
        public delegate void HomeHandler(object sender, RoutedEventArgs e, string nomeAluno);
        public event HomeHandler? SolicitarHome;

        public TelaLogin()
        {
            InitializeComponent();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Password;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: substituir pela validação real no banco de dados
            if (usuario == "admin" && senha == "1234")
            {
                SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuario);
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
            => SolicitarCadastro?.Invoke(this, new RoutedEventArgs());

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
            => SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
    }
}
```

### DEPOIS

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        public event RoutedEventHandler? SolicitarCadastro;
        public event RoutedEventHandler? SolicitarRecuperacaoSenha;

        // Passa o objeto Usuario autenticado após login bem-sucedido
        public delegate void HomeHandler(object sender, RoutedEventArgs e, Usuario usuario);
        public event HomeHandler? SolicitarHome;

        public TelaLogin()
        {
            InitializeComponent();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string codigoAcesso = txtUsuario.Text.Trim();
            string senha = txtSenha.Password;

            if (string.IsNullOrEmpty(codigoAcesso) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dbContext = new LearnixDbContext();
            var authService = new AuthService(dbContext);
            var controller = new LoginController(authService);

            Usuario usuarioAutenticado = authService.RealizarLogin(codigoAcesso, senha);
            if (usuarioAutenticado != null)
            {
                SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuarioAutenticado);
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
            => SolicitarCadastro?.Invoke(this, new RoutedEventArgs());

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
            => SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
    }
}
```

---

## 2. `view/TelaNotas.xaml.cs`

**Commit:** `fix: TelaNotas - implementar DefinirMatricula com Avaliacoes e NotaFinal`

### ANTES

```csharp
using System.Windows.Controls;

namespace Learnix
{
    public partial class TelaNotas : UserControl
    {
        public TelaNotas()
        {
            InitializeComponent();
        }
    }
}
```

### DEPOIS

```csharp
using System.Windows.Controls;
using Learnix.model;

namespace Learnix
{
    public partial class TelaNotas : UserControl
    {
        public TelaNotas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula)
        {
            Sidebar.DefinirAluno(matricula.Aluno?.Nome ?? "Aluno");

            PainelAvaliacoes.Children.Clear();

            foreach (var av in matricula.Avaliacoes)
            {
                var linha = new TextBlock
                {
                    Text = $"{av.Titulo}: {av.Nota:0.0}",
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new System.Windows.Thickness(0, 4, 0, 4)
                };
                PainelAvaliacoes.Children.Add(linha);
            }

            TxtMediaGeral.Text = matricula.NotaFinal.ToString("0.0");
        }
    }
}
```

---

## 3. `view/TelaMeusCursos.xaml.cs`

**Commit:** `fix: TelaMeusCursos - substituir strings por List<Matricula> e cards dinamicos`

### ANTES

```csharp
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
                var p = dados.Split('|');
                var main = Application.Current.MainWindow as MainWindow;
                main?.MostrarAulas(
                    nomeAluno: _nomeAluno,
                    nomeCurso: p.Length > 0 ? p[0] : "",
                    professor: p.Length > 1 ? p[1] : "",
                    categoria: p.Length > 2 ? p[2] : "",
                    cargaHoraria: p.Length > 3 ? p[3] : "",
                    descricao: p.Length > 4 ? p[4] : "",
                    progresso: p.Length > 5 ? p[5] : "0%"
                );
            }
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string dados)
            {
                var p = dados.Split('|');
                string nomeCurso = p.Length > 0 ? p[0] : "";
                string professor = p.Length > 1 ? p[1] : "";
                string cargaHoraria = p.Length > 3 ? p[3] : "";

                TelaCertificados.AdicionarCertificado(_nomeAluno, nomeCurso, professor, cargaHoraria);

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
```

### DEPOIS

```csharp
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
            CarregarCards();
        }

        private void CarregarCards()
        {
            PainelCursos.Children.Clear();
            foreach (var matricula in _matriculas)
            {
                var card = CriarCardMatricula(matricula);
                PainelCursos.Children.Add(card);
            }
        }

        private UIElement CriarCardMatricula(Matricula matricula)
        {
            // Cria card com: matricula.Curso.Titulo, Instrutor.Nome,
            // Categoria.Nome, CargaHoraria, Progresso.PercentualConcluido, Status
            var border = new Border { Margin = new Thickness(0, 0, 0, 12), Tag = matricula };
            // ... (geração dinâmica de UI)
            return border;
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Matricula matricula)
            {
                var main = Application.Current.MainWindow as MainWindow;
                main?.MostrarAulas(matricula);
            }
        }

        private void BtnConcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Matricula matricula)
            {
                var progressoService = new ProgressoService(new LearnixDbContext());
                foreach (var modulo in matricula.Curso?.Modulos ?? new List<Modulo>())
                    foreach (var aula in modulo.Aulas)
                        progressoService.RegistrarConclusaoAula(matricula.Id, aula.Id);

                btn.IsEnabled = false;
                btn.Content = "✔ Concluído";

                string msg = "Parabéns! Você concluiu o curso \"" + matricula.Curso?.Titulo + "\"!\n\n"
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
```

---

## 4. `view/TelaPlayer.xaml.cs`

**Commit:** `fix: TelaPlayer - usar Aula.VideoUrl e registrar ConcluirAula no MediaEnded`

### ANTES

```csharp
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Learnix
{
    public partial class TelaPlayer : UserControl
    {
        private string _nomeAluno = "Aluno";
        private string _nomeCurso = "";
        private bool _isPlaying = false;
        private bool _arrastando = false;
        private DispatcherTimer _timer;

        public TelaPlayer()
        {
            InitializeComponent();
            InicializarTimer();
            VideoPlayer.Volume = SliderVolume.Value;
        }

        public void DefinirAula(string tituloAula, string nomeCurso, string nomeAluno)
        {
            TxtTituloAula.Text = tituloAula;
            TxtNomeCurso.Text = nomeCurso;
            _nomeCurso = nomeCurso;
            _nomeAluno = nomeAluno;
            Sidebar.DefinirAluno(nomeAluno);
            // Sem carregamento de VideoUrl
            // Sem _matriculaId / _aulaId
        }

        // Sem VideoPlayer_MediaEnded
        // ... demais controles do player inalterados
    }
}
```

### DEPOIS

```csharp
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Learnix.Controllers;
using Learnix.data;
using Learnix.model;
using Learnix.Services;

namespace Learnix
{
    public partial class TelaPlayer : UserControl
    {
        private string _nomeAluno = "Aluno";
        private string _nomeCurso = "";
        private bool _isPlaying = false;
        private bool _arrastando = false;
        private DispatcherTimer _timer;
        private int _matriculaId;   // NOVO
        private int _aulaId;        // NOVO

        public TelaPlayer()
        {
            InitializeComponent();
            InicializarTimer();
            VideoPlayer.Volume = SliderVolume.Value;
        }

        public void DefinirAula(Aula aula, Matricula matricula, string nomeAluno)
        {
            TxtTituloAula.Text = aula.Titulo;
            TxtNomeCurso.Text = matricula.Curso?.Titulo ?? "";
            _nomeCurso = matricula.Curso?.Titulo ?? "";
            _nomeAluno = nomeAluno;
            _matriculaId = matricula.Id;
            _aulaId = aula.Id;
            Sidebar.DefinirAluno(nomeAluno);

            if (!string.IsNullOrEmpty(aula.VideoUrl))
            {
                VideoPlayer.Source = new Uri(aula.VideoUrl, UriKind.RelativeOrAbsolute);
                OverlaySemVideo.Visibility = Visibility.Collapsed;
            }
        }

        // NOVO: registra conclusão ao término do vídeo
        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
            _timer.Stop();

            var controller = new AulaController(new ProgressoService(new LearnixDbContext()));
            controller.ConcluirAula(_matriculaId, _aulaId);
        }

        // ... demais controles do player inalterados
    }
}
```

---

## 5. `view/TelaAulas.xaml.cs`

**Commit:** `fix: TelaAulas - receber Matricula e gerar cards de aula dinamicamente`

### ANTES

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = "Aluno";

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirCurso(string nomeCurso, string professor,
            string categoria, string cargaHoraria, string descricao, string progresso)
        {
            TxtNomeCurso.Text = nomeCurso;
            TxtProfessor.Text = professor;
            TxtCategoria.Text = categoria;
            TxtCargaHoraria.Text = cargaHoraria;
            TxtDescricao.Text = descricao;
            TxtProgresso.Text = progresso;
        }

        public void DefinirAluno(string nome)
        {
            _nomeAluno = nome;
            Sidebar.DefinirAluno(nome);
        }

        private void AbrirPlayer(string tituloAula)
        {
            var player = new TelaPlayer();
            player.DefinirAula(tituloAula, TxtNomeCurso.Text, _nomeAluno);
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarTela(player, _nomeAluno);
        }

        private string ObterTituloAula(Border card)
        {
            if (card.Child is Grid grid)
                foreach (var col in grid.Children)
                    if (col is StackPanel sp)
                        foreach (var child in sp.Children)
                            if (child is TextBlock tb && tb.FontSize == 14)
                                return tb.Text;
            return null;
        }
    }
}
```

### DEPOIS

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Learnix.model;

namespace Learnix
{
    public partial class TelaAulas : UserControl
    {
        private string _nomeAluno = "Aluno";
        private Matricula _matricula;

        public TelaAulas()
        {
            InitializeComponent();
        }

        public void DefinirMatricula(Matricula matricula, string nomeAluno)
        {
            _matricula = matricula;
            _nomeAluno = nomeAluno;
            TxtNomeCurso.Text = matricula.Curso?.Titulo;
            TxtProfessor.Text = matricula.Curso?.Instrutor?.Nome;
            TxtCategoria.Text = matricula.Curso?.Categoria?.Nome;
            TxtCargaHoraria.Text = matricula.Curso?.CargaHoraria + "h";
            TxtDescricao.Text = matricula.Curso?.Descricao;
            TxtProgresso.Text = matricula.Progresso?.PercentualConcluido + "%";
            Sidebar.DefinirAluno(nomeAluno);
            CarregarModulos(matricula.Curso?.Modulos);
        }

        private void CarregarModulos(System.Collections.Generic.List<Modulo> modulos)
        {
            PainelModulos.Children.Clear();
            if (modulos == null) return;
            foreach (var modulo in modulos)
            {
                // Cabeçalho do módulo
                PainelModulos.Children.Add(new TextBlock { Text = $"Módulo {modulo.Ordem}: {modulo.Titulo}" });
                foreach (var aula in modulo.Aulas)
                    PainelModulos.Children.Add(CriarCardAula(aula));
            }
        }

        private void AbrirPlayer(Aula aula)
        {
            var player = new TelaPlayer();
            player.DefinirAula(aula, _matricula, _nomeAluno);
            (Application.Current.MainWindow as MainWindow)?.MostrarTela(player, _nomeAluno);
        }
    }
}
```

---

## 6. `view/TelaPerfil.xaml.cs`

**Commit:** `fix: TelaPerfil - exibir MatriculaAcademica, PerfilDeAprendizagem e persistir no banco`

### ANTES

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private bool _modoEdicao = false;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(string nome, string email = "")
        {
            TxtNomePerfil.Text = nome;
            TxtEditNome.Text = nome;
            Sidebar.DefinirAluno(nome);
            // Sem: MatriculaAcademica, EstiloPredominante, RitmoSugerido
            if (!string.IsNullOrEmpty(email))
            {
                TxtEmailPerfil.Text = email;
                TxtEditEmail.Text = email;
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Habilita TxtEditTelefone e TxtEditNascimento (não existem no model!)
            TxtEditTelefone.IsReadOnly = false;
            TxtEditNascimento.IsReadOnly = false;
            // ...
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // ...
            // TODO: persistir no banco de dados  ← nunca implementado
            MessageBox.Show("Perfil atualizado com sucesso!", "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
```

### DEPOIS

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Learnix.data;
using Learnix.model;

namespace Learnix
{
    public partial class TelaPerfil : UserControl
    {
        private bool _modoEdicao = false;
        private Aluno _aluno;

        public TelaPerfil()
        {
            InitializeComponent();
        }

        public void DefinirAluno(Aluno aluno)
        {
            _aluno = aluno;
            TxtNomePerfil.Text = aluno.Nome;
            TxtEditNome.Text = aluno.Nome;
            TxtEditEmail.Text = aluno.Email;
            TxtEmailPerfil.Text = aluno.Email;
            Sidebar.DefinirAluno(aluno.Nome);

            if (TxtMatricula != null)
                TxtMatricula.Text = aluno.MatriculaAcademica;

            if (TxtEstilo != null)
                TxtEstilo.Text = aluno.Perfil?.EstiloPredominante ?? "-";

            if (TxtRitmo != null)
                TxtRitmo.Text = aluno.Perfil?.RitmoSugerido ?? "-";

            if (aluno.Nome.Length > 0)
            {
                var partes = aluno.Nome.Split(' ');
                TxtIniciais.Text = partes.Length >= 2
                    ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                    : aluno.Nome[0].ToString().ToUpper();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Removidos TxtEditTelefone e TxtEditNascimento (não existem no model)
            TxtEditNome.IsReadOnly = false;
            TxtEditEmail.IsReadOnly = false;
            // ...
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // ...
            using var db = new LearnixDbContext();
            var aluno = db.Alunos.Find(_aluno.Id);
            aluno.Nome = TxtEditNome.Text;
            aluno.Email = TxtEditEmail.Text;
            db.SaveChanges(); // persistência real
            MessageBox.Show("Perfil atualizado com sucesso!", "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
```

---

## 7. `Services/AuthService.cs`

**Commit:** `fix: AuthService - substituir login de Instrutor por ID numerico para Email`

### ANTES

```csharp
using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class AuthService : IAuthService
    {
        private readonly LearnixDbContext _context;

        public AuthService(LearnixDbContext context)
        {
            _context = context;
        }

        public Usuario RealizarLogin(string codigoAcesso, string senha)
        {
            Aluno alunoEncontrado = _context.Alunos
                .FirstOrDefault(a => a.MatriculaAcademica == codigoAcesso && a.Senha == senha);

            if (alunoEncontrado != null)
                return alunoEncontrado;

            // Busca instrutor por ID NUMÉRICO (frágil, sem semântica)
            int idInstrutor;
            bool isNumero = int.TryParse(codigoAcesso, out idInstrutor);

            if (isNumero)
            {
                Instrutor instrutorEncontrado = _context.Instrutores
                    .FirstOrDefault(i => i.Id == idInstrutor && i.Senha == senha);

                if (instrutorEncontrado != null)
                    return instrutorEncontrado;
            }

            return null;
        }
    }
}
```

### DEPOIS

```csharp
using System.Linq;
using Learnix.data;
using Learnix.model;

namespace Learnix.Services
{
    public class AuthService : IAuthService
    {
        private readonly LearnixDbContext _context;

        public AuthService(LearnixDbContext context)
        {
            _context = context;
        }

        public Usuario RealizarLogin(string codigoAcesso, string senha)
        {
            Aluno alunoEncontrado = _context.Alunos
                .FirstOrDefault(a => a.MatriculaAcademica == codigoAcesso && a.Senha == senha);

            if (alunoEncontrado != null)
                return alunoEncontrado;

            // Busca instrutor por EMAIL (identificador universal já existente em Usuario)
            Instrutor instrutorEncontrado = _context.Instrutores
                .FirstOrDefault(i => i.Email == codigoAcesso && i.Senha == senha);

            if (instrutorEncontrado != null)
                return instrutorEncontrado;

            return null;
        }
    }
}
```

---

## 8. `view/TelaCertificados.xaml.cs`

**Commit:** `fix: TelaCertificados - remover lista em memoria e usar CarregarDoBanco`

### ANTES

```csharp
// Lista estática em memória — persiste durante toda a sessão
public static List<CertificadoVM> CertificadosSessao { get; } = new();

public void DefinirAluno(string nome)
{
    _nomeAluno = nome;
    SidebarNav?.DefinirAluno(nome);
    // NÃO carregava do banco
}

// Gerava código local, duplicando lógica do ProgressoService
public static void AdicionarCertificado(string nomeAluno, string nomeCurso,
    string professor, string cargaHoraria)
{
    CertificadosSessao.Add(new CertificadoVM
    {
        NomeAluno = nomeAluno,
        NomeCurso = nomeCurso,
        Professor = professor,
        CargaHoraria = cargaHoraria,
        DataConclusao = DateTime.Now.ToString("dd/MM/yyyy"),
        Codigo = "LX-" + Guid.NewGuid().ToString("N")[..6].ToUpper()
    });
}
```

### DEPOIS

```csharp
// Sem CertificadosSessao estática
// Sem AdicionarCertificado

public void DefinirAluno(Learnix.model.Aluno aluno)
{
    _nomeAluno = aluno.Nome;
    SidebarNav?.DefinirAluno(aluno.Nome);

    var certs = aluno.HistoricoMatriculas?
        .Where(m => m.Certificado != null)
        .Select(m => m.Certificado)
        .ToList() ?? new List<CertModel>();

    CarregarDoBanco(certs);
    AtualizarEstado();
}

public void CarregarDoBanco(List<CertModel> certs)
{
    _certificados.Clear();
    foreach (var c in certs)
    {
        _certificados.Add(new CertificadoVM
        {
            NomeAluno     = c.Matricula?.Aluno?.Nome ?? "Aluno",
            NomeCurso     = c.Matricula?.Curso?.Titulo ?? "Curso",
            Professor     = c.Matricula?.Curso?.Instrutor?.Nome ?? "Instrutor",
            CargaHoraria  = (c.Matricula?.Curso?.CargaHoraria.ToString() ?? "0") + "h",
            DataConclusao = c.DataEmissao.ToString("dd/MM/yyyy"),
            Codigo        = c.CodigoCertificado ?? "LX-000000"
        });
    }
}
```

---

## 9. `repositorio/CursoRepository.cs`

**Commit:** `fix: CursoRepository - substituir SQL raw por LINQ com Include TPH`

### ANTES

```csharp
public List<Curso> BuscarCursosPorNome(string termoPesquisa)
{
    string query = "SELECT * FROM Cursos WHERE Titulo LIKE {0}";

    // SQL raw ignora o Discriminator do TPH
    List<Curso> cursos = _context.Cursos
        .FromSqlRaw(query, $"%{termoPesquisa}%")
        .ToList();

    return cursos;
}
```

### DEPOIS

```csharp
public List<Curso> BuscarCursosPorNome(string termoPesquisa)
{
    // LINQ respeitando TPH + eager loading de Categoria e Instrutor
    List<Curso> cursos = _context.Cursos
        .Where(c => c.Titulo.Contains(termoPesquisa))
        .Include(c => c.Categoria)
        .Include(c => c.Instrutor)
        .ToList();

    return cursos;
}
```

---

## 10. `view/TelaMenu.xaml.cs`

**Commit:** `fix: TelaMenu - gerar cards via CursoController e filtrar por categoria`

### ANTES

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        // Cards estáticos: CardCurso1 a CardCurso5

        private void AplicarFiltros()
        {
            string busca = TxtBusca.Text == PlaceholderBusca ? "" : TxtBusca.Text.ToLower().Trim();

            foreach (var card in new[] { CardCurso1, CardCurso2, CardCurso3, CardCurso4, CardCurso5 })
            {
                string categoria = card.Tag?.ToString() ?? "";
                bool passaCategoria = _categoriaAtiva == "Todos" || categoria == _categoriaAtiva;
                bool passaBusca = true;
                if (!string.IsNullOrEmpty(busca))
                {
                    var titulo = EncontrarTitulo(card);
                    passaBusca = titulo != null && titulo.ToLower().Contains(busca);
                }
                card.Visibility = (passaCategoria && passaBusca) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Sem CursoController, sem binding com List<Curso> do banco
    }
}
```

### DEPOIS

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Learnix.Controllers;
using Learnix.data;
using Learnix.Repositorio;

namespace Learnix
{
    public partial class TelaMenu : UserControl
    {
        public TelaMenu()
        {
            InitializeComponent();
            CarregarCursos();
        }

        public void CarregarCursos(string termoBusca = "")
        {
            var controller = new CursoController(new CursoRepository(new LearnixDbContext()));
            var cursos = controller.BuscarCursos(termoBusca);
            ListaCursos.ItemsSource = cursos;
        }

        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtBusca.Text == PlaceholderBusca) return;
            CarregarCursos(TxtBusca.Text.Trim());
        }

        private void AplicarFiltroCategoria()
        {
            if (ListaCursos.ItemsSource is System.Collections.Generic.List<Learnix.model.Curso> cursos)
            {
                var view = System.Windows.Data.CollectionViewSource.GetDefaultView(cursos);
                view.Filter = item =>
                {
                    if (_categoriaAtiva == "Todos") return true;
                    if (item is Learnix.model.Curso curso)
                        return curso.Categoria?.Nome == _categoriaAtiva;
                    return false;
                };
                view.Refresh();
            }
        }
    }
}
```

---

*Backup gerado em 24/05/2026 — Learnix*
