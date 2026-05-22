# Learnix — Telas de Autenticação (WPF UserControl)

Documentação das telas de Login, Cadastro e Recuperação de Senha do projeto **Learnix**, desenvolvidas em C# WPF UserControl.

---

## Estrutura de Arquivos

```
Learnix/
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── TelaLogin.xaml
├── TelaLogin.xaml.cs
├── TelaCadastro.xaml
├── TelaCadastro.xaml.cs
├── TelaEsqueceuSenha.xaml
└── TelaEsqueceuSenha.xaml.cs
```

---

## Fluxo de Navegação

```
TelaLogin
  ├── [Entrar]            → valida credenciais (TODO: banco de dados)
  ├── [Esqueceu a senha?] → TelaEsqueceuSenha → [← Voltar] → TelaLogin
  └── [Cadastre-se]       → TelaCadastro      → [← Voltar] → TelaLogin
```

---

## 1. MainWindow

### `MainWindow.xaml`

```xml
<Window x:Class="Learnix.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:Learnix"
        Title="Learnix" Height="480" Width="500"
        WindowStartupLocation="CenterScreen">
    <Grid>
        <ContentControl x:Name="conteudoPrincipal"/>
    </Grid>
</Window>
```

### `MainWindow.xaml.cs`

```csharp
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

        private void MostrarLogin()
        {
            var tela = new TelaLogin();
            tela.SolicitarCadastro += (s, e) => MostrarCadastro();
            tela.SolicitarRecuperacaoSenha += (s, e) => MostrarEsqueceuSenha();
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
    }
}
```

---

## 2. Tela de Login
<UserControl x:Class="Learnix.TelaLogin"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Background="#1E1E1E"
             Width="500" Height="420">

    <UserControl.Resources>

        <Style x:Key="InputStyle" TargetType="TextBox">
            <Setter Property="Height" Value="34"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#F5F5F0"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="12" Padding="{TemplateBinding Padding}">
                            <ScrollViewer x:Name="PART_ContentHost" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="PasswordStyle" TargetType="PasswordBox">
            <Setter Property="Height" Value="34"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#F5F5F0"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="PasswordBox">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="12" Padding="{TemplateBinding Padding}">
                            <ScrollViewer x:Name="PART_ContentHost" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="BotaoEntrarStyle" TargetType="Button">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#4E3A7A"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="borda" Background="{TemplateBinding Background}" CornerRadius="12">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="borda" Property="Background" Value="#3A2860"/>
                            </Trigger>
                            <Trigger Property="IsPressed" Value="True">
                                <Setter TargetName="borda" Property="Background" Value="#2C1F4A"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="LinkStyle" TargetType="TextBlock">
            <Setter Property="Foreground" Value="#D8CCF0"/>
            <Setter Property="FontSize" Value="11"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="TextDecorations" Value="Underline"/>
            <Setter Property="HorizontalAlignment" Value="Center"/>
        </Style>

    </UserControl.Resources>

    <Grid HorizontalAlignment="Center" VerticalAlignment="Center">
        <Border Background="#7E6BAC" CornerRadius="8"
                Width="400" Height="340" Padding="20">

            <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">

                <StackPanel HorizontalAlignment="Center" Margin="0,0,0,16">
                    <TextBlock Text="🎓" FontSize="32" HorizontalAlignment="Center"/>
                    <TextBlock Text="LEARNIX" FontSize="11" FontWeight="Bold"
                               Foreground="#3C3250" HorizontalAlignment="Center"/>
                </StackPanel>

                <TextBlock Text="Usuário:" Foreground="WhiteSmoke" FontSize="12" Margin="0,0,0,4"/>
                <TextBox x:Name="txtUsuario" Style="{StaticResource InputStyle}" Margin="0,0,0,6"/>

                <TextBlock Text="Senha:" Foreground="WhiteSmoke" FontSize="12" Margin="0,4,0,4"/>
                <PasswordBox x:Name="txtSenha" Style="{StaticResource PasswordStyle}" Margin="0,0,0,6"/>

                <TextBlock x:Name="lnkEsqueceuSenha"
                           Text="Esqueceu a senha?"
                           Style="{StaticResource LinkStyle}"
                           Margin="0,4,0,0"
                           MouseLeftButtonDown="LnkEsqueceuSenha_Click"/>

                <Button x:Name="btnEntrar" Content="Entrar"
                        Style="{StaticResource BotaoEntrarStyle}"
                        Margin="0,12,0,0"
                        Click="BtnEntrar_Click"/>

                <Separator Background="#9E8FC0" Margin="0,10,0,6"/>

                <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                    <TextBlock Text="Não tem conta?" Foreground="#D8CCF0" FontSize="11" Margin="0,0,4,0"/>
                    <TextBlock x:Name="lnkCadastro"
                               Text="Cadastre-se"
                               Style="{StaticResource LinkStyle}"
                               MouseLeftButtonDown="LnkCadastro_Click"/>
                </StackPanel>

            </StackPanel>

        </Border>
    </Grid>

</UserControl>


### `TelaLogin.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaLogin : UserControl
    {
        public event RoutedEventHandler SolicitarCadastro;
        public event RoutedEventHandler SolicitarRecuperacaoSenha;

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

            // TODO: validação real (banco de dados / serviço)
            if (usuario == "admin" && senha == "1234")
            {
                MessageBox.Show("Login realizado com sucesso!",
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navegar para tela principal aqui
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LnkCadastro_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarCadastro?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkEsqueceuSenha_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarRecuperacaoSenha?.Invoke(this, new RoutedEventArgs());
        }
    }
}
```

---

## 3. Tela de Cadastro

<UserControl x:Class="Learnix.TelaCadastro"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Background="#1E1E1E"
             Width="500" Height="480">

    <UserControl.Resources>

        <Style x:Key="InputStyle" TargetType="TextBox">
            <Setter Property="Height" Value="34"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#F5F5F0"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="12" Padding="{TemplateBinding Padding}">
                            <ScrollViewer x:Name="PART_ContentHost" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="PasswordStyle" TargetType="PasswordBox">
            <Setter Property="Height" Value="34"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#F5F5F0"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="PasswordBox">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="12" Padding="{TemplateBinding Padding}">
                            <ScrollViewer x:Name="PART_ContentHost" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="BotaoStyle" TargetType="Button">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#4E3A7A"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="borda" Background="{TemplateBinding Background}" CornerRadius="12">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="borda" Property="Background" Value="#3A2860"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

    </UserControl.Resources>

    <Grid HorizontalAlignment="Center" VerticalAlignment="Center">
        <Border Background="#7E6BAC" CornerRadius="8"
                Width="400" Height="400" Padding="20">

            <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">

                <StackPanel HorizontalAlignment="Center" Margin="0,0,0,12">
                    <TextBlock Text="🎓" FontSize="28" HorizontalAlignment="Center"/>
                    <TextBlock Text="LEARNIX" FontSize="11" FontWeight="Bold"
                               Foreground="#3C3250" HorizontalAlignment="Center"/>
                </StackPanel>

                <TextBlock Text="Criar conta" FontSize="15" FontWeight="Bold"
                           Foreground="White" HorizontalAlignment="Center"
                           Margin="0,0,0,8"/>

                <TextBlock Text="Nome completo:" Foreground="WhiteSmoke" FontSize="12" Margin="0,0,0,4"/>
                <TextBox x:Name="txtNome" Style="{StaticResource InputStyle}" Margin="0,0,0,6"/>

                <TextBlock Text="E-mail:" Foreground="WhiteSmoke" FontSize="12" Margin="0,8,0,4"/>
                <TextBox x:Name="txtEmail" Style="{StaticResource InputStyle}" Margin="0,0,0,6"/>

                <TextBlock Text="Senha:" Foreground="WhiteSmoke" FontSize="12" Margin="0,8,0,4"/>
                <PasswordBox x:Name="txtSenha" Style="{StaticResource PasswordStyle}" Margin="0,0,0,6"/>

                <TextBlock Text="Confirmar senha:" Foreground="WhiteSmoke" FontSize="12" Margin="0,8,0,4"/>
                <PasswordBox x:Name="txtConfirmarSenha" Style="{StaticResource PasswordStyle}" Margin="0,0,0,6"/>

                <Button Content="Cadastrar" Style="{StaticResource BotaoStyle}"
                        Margin="0,14,0,0" Click="BtnCadastrar_Click"/>

                <TextBlock Text="← Voltar ao login"
                           Foreground="#D8CCF0" FontSize="11"
                           Cursor="Hand" TextDecorations="Underline"
                           HorizontalAlignment="Center" Margin="0,6,0,0"
                           MouseLeftButtonDown="LnkVoltar_Click"/>

            </StackPanel>

        </Border>
    </Grid>

</UserControl>

### `TelaCadastro.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaCadastro : UserControl
    {
        public event RoutedEventHandler SolicitarLogin;

        public TelaCadastro()
        {
            InitializeComponent();
        }

        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Password;
            string confirmar = txtConfirmarSenha.Password;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(confirmar))
            {
                MessageBox.Show("Preencha todos os campos.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (senha != confirmar)
            {
                MessageBox.Show("As senhas não coincidem.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: salvar usuário no banco de dados
            MessageBox.Show("Cadastro realizado com sucesso! Faça login.", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}
```

---

## 4. Tela de Recuperação de Senha

<UserControl x:Class="Learnix.TelaEsqueceuSenha"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Background="#1E1E1E"
             Width="500" Height="420">

    <UserControl.Resources>

        <Style x:Key="InputStyle" TargetType="TextBox">
            <Setter Property="Height" Value="34"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#F5F5F0"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="12" Padding="{TemplateBinding Padding}">
                            <ScrollViewer x:Name="PART_ContentHost" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="BotaoStyle" TargetType="Button">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Width" Value="230"/>
            <Setter Property="Background" Value="#4E3A7A"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="borda" Background="{TemplateBinding Background}" CornerRadius="12">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="borda" Property="Background" Value="#3A2860"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

    </UserControl.Resources>

    <Grid HorizontalAlignment="Center" VerticalAlignment="Center">
        <Border Background="#7E6BAC" CornerRadius="8"
                Width="400" Height="300" Padding="20">

            <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">

                <StackPanel HorizontalAlignment="Center" Margin="0,0,0,12">
                    <TextBlock Text="🎓" FontSize="28" HorizontalAlignment="Center"/>
                    <TextBlock Text="LEARNIX" FontSize="11" FontWeight="Bold"
                               Foreground="#3C3250" HorizontalAlignment="Center"/>
                </StackPanel>

                <TextBlock Text="Recuperar senha" FontSize="15" FontWeight="Bold"
                           Foreground="White" HorizontalAlignment="Center"
                           Margin="0,0,0,4"/>

                <TextBlock Text="Informe seu e-mail cadastrado e enviaremos&#10;as instruções de recuperação."
                           Foreground="#E0D8F5" FontSize="11"
                           TextAlignment="Center" HorizontalAlignment="Center"
                           Margin="0,0,0,12"/>

                <TextBlock Text="E-mail:" Foreground="WhiteSmoke" FontSize="12" Margin="0,0,0,4"/>
                <TextBox x:Name="txtEmail" Style="{StaticResource InputStyle}" Margin="0,0,0,6"/>

                <Button Content="Enviar instruções" Style="{StaticResource BotaoStyle}"
                        Margin="0,16,0,0" Click="BtnEnviar_Click"/>

                <TextBlock Text="← Voltar ao login"
                           Foreground="#D8CCF0" FontSize="11"
                           Cursor="Hand" TextDecorations="Underline"
                           HorizontalAlignment="Center" Margin="0,8,0,0"
                           MouseLeftButtonDown="LnkVoltar_Click"/>

            </StackPanel>

        </Border>
    </Grid>

</UserControl>
### `TelaEsqueceuSenha.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Learnix
{
    public partial class TelaEsqueceuSenha : UserControl
    {
        public event RoutedEventHandler SolicitarLogin;

        public TelaEsqueceuSenha()
        {
            InitializeComponent();
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Informe seu e-mail.", "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: integrar com serviço de e-mail / back-end
            MessageBox.Show($"Se o e-mail '{email}' estiver cadastrado, você receberá as instruções em breve.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);

            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }

        private void LnkVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            SolicitarLogin?.Invoke(this, new RoutedEventArgs());
        }
    }
}
```

---

## Paleta de Cores

| Elemento            | Cor HEX   |
|---------------------|-----------|
| Fundo da janela     | `#1E1E1E` |
| Painel central      | `#7E6BAC` |
| Botão principal     | `#4E3A7A` |
| Botão hover         | `#3A2860` |
| Botão pressed       | `#2C1F4A` |
| Texto logo          | `#3C3250` |
| Inputs              | `#F5F5F0` |
| Links               | `#D8CCF0` |
| Separador           | `#9E8FC0` |

---

## Observações

- A navegação entre telas é controlada pela `MainWindow` via **eventos customizados**, sem acoplamento direto entre os UserControls.
- Os campos de senha utilizam `PasswordBox` nativo do WPF, garantindo segurança no input.
- Os blocos marcados com `// TODO` indicam onde deve ser integrado o **banco de dados ou serviço de autenticação**.
