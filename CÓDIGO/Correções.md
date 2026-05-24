# Learnix — Plano de Correções

> Todas as correções necessárias para alinhar o projeto ao padrão MVC e às classes do `model`.
> Organizado por prioridade: 🔴 Alta → 🟡 Média → 🟢 Baixa.

---

## 🔴 Prioridade Alta

---

### 1. `view/TelaLogin.xaml.cs` — Login hardcoded sem uso do AuthService

**Problema:**
O login está validado com `if (usuario == "admin" && senha == "1234")`, completamente fora do model e sem passar pelo `LoginController` ou `AuthService`. Após o login, apenas uma `string` com o nome é propagada, em vez de um objeto `Usuario`.

**O que corrigir:**

1. Instanciar (ou receber via injeção) o `LoginController` passando um `AuthService`.
2. Substituir o bloco `if (usuario == "admin")` pela chamada real:
```csharp
   // Antes (errado)
   if (usuario == "admin" && senha == "1234")
   {
       SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuario);
   }

   // Depois (correto)
   var dbContext = new LearnixDbContext();
   var authService = new AuthService(dbContext);
   var controller = new LoginController(authService);

   Usuario usuarioAutenticado = authService.RealizarLogin(txtUsuario.Text, txtSenha.Password);
   if (usuarioAutenticado != null)
   {
       SolicitarHome?.Invoke(this, new RoutedEventArgs(), usuarioAutenticado);
   }
```
3. Alterar a assinatura do evento `SolicitarHome` para passar `Usuario` em vez de `string`:
```csharp
   // Antes
   public delegate void HomeHandler(object sender, RoutedEventArgs e, string nomeAluno);

   // Depois
   public delegate void HomeHandler(object sender, RoutedEventArgs e, Usuario usuario);
```

---

### 2. `view/TelaNotas.xaml.cs` — Classe completamente vazia

**Problema:**
A classe só tem `InitializeComponent()`. Não consome `Matricula.Avaliacoes`, `Avaliacao.Titulo`, `Avaliacao.Nota` nem `Matricula.NotaFinal`.

**O que corrigir:**

1. Adicionar um método `DefinirMatricula(Matricula matricula)` que popule a tabela de notas:
```csharp
   public void DefinirMatricula(Matricula matricula)
   {
       // Popular as linhas AV1, AV2, AV3 da tabela
       foreach (var av in matricula.Avaliacoes)
       {
           // Bind com Avaliacao.Titulo e Avaliacao.Nota
       }
       // Exibir a média
       TxtMediaGeral.Text = matricula.NotaFinal.ToString("0.0");
   }
```
2. Adicionar o campo `Sidebar.DefinirAluno(nome)` para manter a navbar consistente.
3. Garantir que a `MainWindow` chame `telaNotas.DefinirMatricula(matricula)` ao navegar para essa tela.

---

### 3. `view/TelaMeusCursos.xaml.cs` — Dados passados como string concatenada

**Problema:**
Os dados dos cursos são passados via `Tag` no formato `"NomeCurso|Professor|Categoria|CargaHoraria|Descricao|Progresso"`. Isso ignora completamente as classes `Matricula`, `Curso` e `Progresso` do model. O botão "Concluir" emite o certificado diretamente sem verificar `Progresso.PercentualConcluido` nem usar `StatusMatricula`.

**O que corrigir:**

1. Substituir a propriedade `Tag` (string) por uma lista de `Matricula` carregada via serviço:
```csharp
   private List<Matricula> _matriculas = new();

   public void DefinirAluno(Aluno aluno)
   {
       _nomeAluno = aluno.Nome;
       _matriculas = aluno.HistoricoMatriculas;
       Sidebar.DefinirAluno(aluno.Nome);
       CarregarCards();
   }
```
2. Gerar os cards de curso dinamicamente a partir de `_matriculas`, usando:
   - `matricula.Curso.Titulo`
   - `matricula.Curso.Instrutor.Nome`
   - `matricula.Curso.Categoria.Nome`
   - `matricula.Curso.CargaHoraria`
   - `matricula.Progresso.PercentualConcluido`
   - `matricula.Status` para habilitar/desabilitar o botão "Concluir"

3. No botão "Concluir", chamar o serviço em vez de gerar o certificado direto:
```csharp
   // Antes (errado)
   TelaCertificados.AdicionarCertificado(_nomeAluno, nomeCurso, professor, cargaHoraria);

   // Depois (correto) — o ProgressoService já cuida de emitir o certificado
   var progressoService = new ProgressoService(new LearnixDbContext());
   progressoService.RegistrarConclusaoAula(matriculaId, aulaId);
```

4. Ao navegar para `TelaAulas`, passar o objeto `Matricula` em vez de strings:
```csharp
   main?.MostrarAulas(matricula); // em vez de strings separadas
```

---

## 🟡 Prioridade Média

---

### 4. `view/TelaPlayer.xaml.cs` — `Aula.VideoUrl` ignorado e conclusão de aula não registrada

**Problema:**
O vídeo só é carregado via `OpenFileDialog` manual. O campo `Aula.VideoUrl` do model nunca é usado. Quando o aluno termina de assistir, `AulaController.ConcluirAula` não é chamado, então `Progresso` nunca é atualizado.

**O que corrigir:**

1. Alterar `DefinirAula` para receber o objeto `Aula` completo:
```csharp
   // Antes
   public void DefinirAula(string tituloAula, string nomeCurso, string nomeAluno)

   // Depois
   public void DefinirAula(Aula aula, Matricula matricula, string nomeAluno)
   {
       TxtTituloAula.Text = aula.Titulo;
       TxtNomeCurso.Text = matricula.Curso.Titulo;
       _nomeAluno = nomeAluno;
       _matriculaId = matricula.Id;
       _aulaId = aula.Id;
       Sidebar.DefinirAluno(nomeAluno);

       // Carrega o vídeo automaticamente pelo URL do model
       if (!string.IsNullOrEmpty(aula.VideoUrl))
           VideoPlayer.Source = new Uri(aula.VideoUrl, UriKind.RelativeOrAbsolute);
   }
```

2. Ao término do vídeo (`VideoPlayer.MediaEnded`), registrar a conclusão:
```csharp
   private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
   {
       var controller = new AulaController(new ProgressoService(new LearnixDbContext()));
       controller.ConcluirAula(_matriculaId, _aulaId);
   }
```

---

### 5. `view/TelaAulas.xaml.cs` — Recebe strings em vez de objetos do model

**Problema:**
`DefinirCurso(...)` recebe 6 parâmetros de string avulsos. Ao abrir o player, passa apenas `string tituloAula`, sem o objeto `Aula` (e portanto sem `VideoUrl`).

**O que corrigir:**

1. Substituir `DefinirCurso(string, string, string, string, string, string)` por:
```csharp
   public void DefinirMatricula(Matricula matricula, string nomeAluno)
   {
       _matricula = matricula;
       _nomeAluno = nomeAluno;
       TxtNomeCurso.Text = matricula.Curso.Titulo;
       TxtProfessor.Text = matricula.Curso.Instrutor.Nome;
       TxtCategoria.Text = matricula.Curso.Categoria.Nome;
       TxtCargaHoraria.Text = matricula.Curso.CargaHoraria + "h";
       TxtDescricao.Text = matricula.Curso.Descricao;
       TxtProgresso.Text = matricula.Progresso.PercentualConcluido + "%";
       CarregarModulos(matricula.Curso.Modulos);
   }
```

2. Gerar os cards de aula dinamicamente iterando `Modulo.Aulas`:
```csharp
   foreach (var modulo in matricula.Curso.Modulos)
   {
       // Criar cabeçalho de seção com modulo.Titulo e modulo.Ordem
       foreach (var aula in modulo.Aulas)
       {
           // Criar card com aula.Titulo, aula.Duracao, aula.Ordem
           // Botão "Assistir" passa objeto aula para AbrirPlayer(aula)
       }
   }
```

3. Ao abrir o player, passar o objeto `Aula`:
```csharp
   private void AbrirPlayer(Aula aula)
   {
       var player = new TelaPlayer();
       player.DefinirAula(aula, _matricula, _nomeAluno);
       (Application.Current.MainWindow as MainWindow)?.MostrarTela(player, _nomeAluno);
   }
```

---

### 6. `view/TelaPerfil.xaml.cs` — Campos sem correspondência no model e dados incompletos

**Problema:**
Os campos `TxtEditTelefone` e `TxtEditNascimento` não existem no model. `Aluno.MatriculaAcademica` e `PerfilDeAprendizagem` (`EstiloPredominante`, `RitmoSugerido`) nunca são exibidos. A edição não é salva (`// TODO: persistir no banco`).

**O que corrigir:**

1. Alterar `DefinirAluno` para receber o objeto `Aluno`:
```csharp
   public void DefinirAluno(Aluno aluno)
   {
       TxtNomePerfil.Text = aluno.Nome;
       TxtEditNome.Text = aluno.Nome;
       TxtEditEmail.Text = aluno.Email;
       TxtMatricula.Text = aluno.MatriculaAcademica;        // campo a adicionar no XAML
       TxtEstilo.Text = aluno.Perfil?.EstiloPredominante;   // campo a adicionar no XAML
       TxtRitmo.Text = aluno.Perfil?.RitmoSugerido;         // campo a adicionar no XAML
       Sidebar.DefinirAluno(aluno.Nome);
   }
```

2. Remover `TxtEditTelefone` e `TxtEditNascimento` do XAML (não existem no model), **ou** adicionar `Telefone` e `DataNascimento` à classe `Usuario` no model caso queiram manter esses campos.

3. No `BtnSalvar_Click`, persistir as alterações:
```csharp
   // Substituir o TODO por:
   using var db = new LearnixDbContext();
   var aluno = db.Alunos.Find(_aluno.Id);
   aluno.Nome = TxtEditNome.Text;
   aluno.Email = TxtEditEmail.Text;
   db.SaveChanges();
```

---

### 7. `Services/AuthService.cs` — Login de instrutor por ID numérico sem campo dedicado

**Problema:**
O instrutor faz login usando seu `Id` numérico como "código de acesso". O model não define nenhum campo como `RegistroInstrutor` ou `CodigoAcesso`, tornando esse fluxo frágil e sem semântica clara.

**O que corrigir:**

**Opção A** — Adicionar `RegistroInstrutor` ao model `Instrutor`:
```csharp
// Em model/Instrutor.cs
public string RegistroInstrutor { get; set; } = null!;
// Ex: "INST-0042"
```
E atualizar o `AuthService` para buscar por esse campo em vez de `Id`.

**Opção B** — Usar `Email` como identificador universal (já existe em `Usuario`):
```csharp
// Em AuthService.cs
var usuario = _context.Usuarios
    .FirstOrDefault(u => u.Email == codigoAcesso && u.Senha == senha);
return usuario;
```

---

## 🟢 Prioridade Baixa

---

### 8. `view/TelaCertificados.xaml.cs` — Duplicação da lógica de geração de código

**Problema:**
`AdicionarCertificado` gera o código `"LX-" + Guid...` localmente, duplicando a lógica que já existe em `ProgressoService` (que gera `Guid.NewGuid().ToString().Substring(0,8).ToUpper()`). A lista em memória (`CertificadosSessao`) substitui o banco indevidamente.

**O que corrigir:**

1. Remover `AdicionarCertificado` e `CertificadosSessao`.
2. Chamar `CarregarDoBanco` passando os certificados reais do aluno logado:
```csharp
   public void DefinirAluno(Aluno aluno)
   {
       _nomeAluno = aluno.Nome;
       SidebarNav?.DefinirAluno(aluno.Nome);

       var certs = aluno.HistoricoMatriculas
           .Where(m => m.Certificado != null)
           .Select(m => m.Certificado)
           .ToList();

       TelaCertificados.CarregarDoBanco(certs);
       AtualizarEstado();
   }
```

---

### 9. `repositorio/CursoRepository.cs` — SQL raw com TPH

**Problema:**
A query `SELECT * FROM Cursos WHERE Titulo LIKE {0}` usa SQL raw. Como `CursoExatas` e `CursoHumanas` usam Table-Per-Hierarchy (TPH), o EF adiciona automaticamente uma coluna discriminadora (`Discriminator`). O SQL raw ignora esse mapeamento e pode trazer objetos do tipo base `Curso` em vez das subclasses corretas.

**O que corrigir:**

Substituir SQL raw por LINQ:
```csharp
// Antes
List<Curso> cursos = _context.Cursos
    .FromSqlRaw(query, $"%{termoPesquisa}%")
    .ToList();

// Depois
List<Curso> cursos = _context.Cursos
    .Where(c => c.Titulo.Contains(termoPesquisa))
    .Include(c => c.Categoria)
    .Include(c => c.Instrutor)
    .ToList();
```

---

### 10. `view/TelaMenu.xaml.cs` — Cards de curso estáticos no XAML

**Problema:**
Os cursos são representados por 5 cards fixos (`CardCurso1` a `CardCurso5`) no XAML com dados hardcoded. Não há binding com `List<Curso>` carregada via `CursoController`.

**O que corrigir:**

1. Remover os 5 cards fixos do XAML e substituir por um `ItemsControl` ou `WrapPanel` com `DataTemplate`.
2. No code-behind, carregar os cursos via controller:
```csharp
   public void CarregarCursos()
   {
       var controller = new CursoController(new CursoRepository(new LearnixDbContext()));
       var cursos = controller.BuscarCursos("");
       ListaCursos.ItemsSource = cursos; // binding com o ItemsControl
   }
```
3. No `DataTemplate`, bindar os campos:
   - `{Binding Titulo}`
   - `{Binding Descricao}`
   - `{Binding Preco}`
   - `{Binding CargaHoraria}`
   - `{Binding Instrutor.Nome}`
   - `{Binding Categoria.Nome}`

---

## Checklist Geral

| # | Arquivo                          | Correção                                              | Status |
|---|----------------------------------|-------------------------------------------------------|--------|
| 1 | `view/TelaLogin.xaml.cs`         | Integrar `AuthService` e propagar objeto `Usuario`    | ⬜     |
| 2 | `view/TelaNotas.xaml.cs`         | Implementar `DefinirMatricula` com `Avaliacoes`       | ⬜     |
| 3 | `view/TelaMeusCursos.xaml.cs`    | Substituir strings por `List<Matricula>`              | ⬜     |
| 4 | `view/TelaPlayer.xaml.cs`        | Usar `Aula.VideoUrl` e chamar `ConcluirAula`          | ⬜     |
| 5 | `view/TelaAulas.xaml.cs`         | Receber `Matricula` e gerar cards dinamicamente       | ⬜     |
| 6 | `view/TelaPerfil.xaml.cs`        | Exibir `MatriculaAcademica` e `PerfilDeAprendizagem`  | ⬜     |
| 7 | `Services/AuthService.cs`        | Padronizar identificador de login do `Instrutor`      | ⬜     |
| 8 | `view/TelaCertificados.xaml.cs`  | Remover lista em memória e usar `CarregarDoBanco`     | ⬜     |
| 9 | `repositorio/CursoRepository.cs` | Substituir SQL raw por LINQ com `.Include()`          | ⬜     |
|10 | `view/TelaMenu.xaml.cs`          | Gerar cards de curso dinamicamente via `CursoController` | ⬜  |

---

*Learnix — gerado em 24/05/2026*
