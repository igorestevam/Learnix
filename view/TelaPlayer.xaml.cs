using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Learnix.data;
using Learnix.model;
using Learnix.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Learnix
{
    public partial class TelaPlayer : UserControl
    {
        private string _nomeAluno = string.Empty;
        private bool _isPlaying = false;
        private bool _arrastando = false;
        private DispatcherTimer? _timer;
        private int _matriculaId;
        private int _aulaId;
        private Matricula? _matricula;
        private List<Aula> _aulas = new();
        private int _aulaAtualIndex = 0;

        public SidebarControl SidebarPublic => Sidebar;

        public TelaPlayer()
        {
            InitializeComponent();
            InicializarTimer();
            VolumePadrao();
        }

        private void VolumePadrao()
        {
            if (VideoPlayer != null) VideoPlayer.Volume = 0.7;
            VideoPlayer.Volume = 0.8;
            VideoPlayer.IsMuted = false;
        }

        public void DefinirAula(Matricula matricula, Aula aula, string nomeAluno = "")
        {
            if (!string.IsNullOrWhiteSpace(nomeAluno))
            {
                _nomeAluno = nomeAluno;
                Sidebar?.DefinirAluno(nomeAluno);
            }

            _matriculaId = matricula.Id;
            _aulaId = aula.Id;

            // Recarrega a matrícula completa do banco
            using var db = new LearnixDbContext();
            _matricula = db.Matriculas
                .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(mod => mod.Aulas)
                .FirstOrDefault(m => m.Id == matricula.Id) ?? matricula;

            if (string.IsNullOrWhiteSpace(_nomeAluno) && matricula.Aluno != null)
            {
                _nomeAluno = matricula.Aluno.Nome;
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);
            }

            TxtTituloAula.Text = aula.Titulo ?? "Aula";
            TxtNomeCurso.Text = _matricula.Curso?.Titulo ?? "";

            // Monta lista de aulas
            _aulas = _matricula.Curso?.Modulos?
                .OrderBy(m => m.Ordem)
                .SelectMany(m => m.Aulas.OrderBy(a => a.Ordem))
                .ToList() ?? new List<Aula>();

            _aulaAtualIndex = _aulas.FindIndex(a => a.Id == aula.Id);
            if (_aulaAtualIndex < 0) _aulaAtualIndex = 0;

            // Carrega aulas concluídas e renderiza lista lateral
            var aulasConcluidas = db.AulasConcluidas
                .Where(ac => ac.MatriculaId == _matriculaId)
                .Select(ac => ac.AulaId)
                .ToHashSet();

            RenderizarListaAulas(aulasConcluidas);

            // Carrega vídeo se tiver URL
            if (!string.IsNullOrWhiteSpace(aula.VideoUrl))
            {
                try
                {
                    VideoPlayer.Source = new Uri(aula.VideoUrl, UriKind.RelativeOrAbsolute);
                    OverlaySemVideo.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    OverlaySemVideo.Visibility = Visibility.Visible;
                }
            }
            else
            {
                OverlaySemVideo.Visibility = Visibility.Visible;
            }
        }

        private void RenderizarListaAulas(HashSet<int> aulasConcluidas)
        {
            // Limpa lista lateral
            ListaAulasPanel.Children.Clear();

            string moduloAtual = "";
            for (int i = 0; i < _aulas.Count; i++)
            {
                var aula = _aulas[i];
                var modulo = _matricula?.Curso?.Modulos?
                    .FirstOrDefault(m => m.Aulas.Any(a => a.Id == aula.Id));

                if (modulo != null && modulo.Titulo != moduloAtual)
                {
                    moduloAtual = modulo.Titulo;
                    ListaAulasPanel.Children.Add(new TextBlock
                    {
                        Text = modulo.Titulo,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0")),
                        FontSize = 11,
                        FontWeight = FontWeights.SemiBold,
                        FontFamily = new FontFamily("Segoe UI"),
                        Margin = new Thickness(8, 12, 0, 4),
                    });
                }

                bool concluida = aulasConcluidas.Contains(aula.Id);
                bool eAtual = i == _aulaAtualIndex;
                bool bloqueada = !concluida && i > _aulaAtualIndex;

                string iconeBg = concluida ? "#1B5E20" : eAtual ? "#4E3A7A" : "#3A2860";
                string icone = concluida ? "✔" : eAtual ? "▶" : "🔒";
                string iconeFg = concluida ? "#A5D6A7" : "White";
                string nomeFg = eAtual ? "White" : concluida ? "#D8CCF0" : "#9E8FC0";
                string statusTxt = eAtual ? $"{aula.Duracao.Minutes} min • Em andamento" : $"{aula.Duracao.Minutes} min";
                string statusFg = eAtual ? "#FFCC80" : "#9E8FC0";

                var item = new Border
                {
                    Padding = new Thickness(14, 12, 14, 12),
                    Margin = new Thickness(0, 0, 0, 2),
                    CornerRadius = new CornerRadius(8),
                    Background = eAtual
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A2860"))
                        : new SolidColorBrush(Colors.Transparent),
                    Opacity = bloqueada ? 0.45 : 1.0,
                    Cursor = bloqueada ? Cursors.Arrow : Cursors.Hand,
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var iconeBorder = new Border
                {
                    Width = 28,
                    Height = 28,
                    CornerRadius = new CornerRadius(14),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(iconeBg)),
                    Margin = new Thickness(0, 0, 10, 0),
                };
                iconeBorder.Child = new TextBlock
                {
                    Text = icone,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(iconeFg)),
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(iconeBorder, 0);

                var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                info.Children.Add(new TextBlock
                {
                    Text = $"Aula {i + 1:D2} — {aula.Titulo}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(nomeFg)),
                    FontSize = 12,
                    FontWeight = eAtual ? FontWeights.SemiBold : FontWeights.Normal,
                    FontFamily = new FontFamily("Segoe UI"),
                    TextWrapping = TextWrapping.Wrap,
                });
                info.Children.Add(new TextBlock
                {
                    Text = statusTxt,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(statusFg)),
                    FontSize = 11,
                    FontFamily = new FontFamily("Segoe UI"),
                });
                Grid.SetColumn(info, 1);

                grid.Children.Add(iconeBorder);
                grid.Children.Add(info);
                item.Child = grid;

                // Clique na aula da lista
                if (!bloqueada)
                {
                    int capturedIndex = i;
                    var capturedAula = aula;
                    item.MouseLeftButtonDown += (s, e) =>
                    {
                        _aulaAtualIndex = capturedIndex;
                        _aulaId = capturedAula.Id;
                        TxtTituloAula.Text = capturedAula.Titulo ?? "Aula";

                        if (!string.IsNullOrWhiteSpace(capturedAula.VideoUrl))
                        {
                            try
                            {
                                VideoPlayer.Source = new Uri(capturedAula.VideoUrl, UriKind.RelativeOrAbsolute);
                                VideoPlayer.Play();
                                _isPlaying = true;
                                BtnPlayPause.Content = "⏸";
                                OverlaySemVideo.Visibility = Visibility.Collapsed;
                            }
                            catch { OverlaySemVideo.Visibility = Visibility.Visible; }
                        }
                        else
                        {
                            VideoPlayer.Source = null;
                            OverlaySemVideo.Visibility = Visibility.Visible;
                        }

                        using var db = new LearnixDbContext();
                        var concluidas = db.AulasConcluidas
                            .Where(ac => ac.MatriculaId == _matriculaId)
                            .Select(ac => ac.AulaId)
                            .ToHashSet();
                        RenderizarListaAulas(concluidas);
                    };
                }

                ListaAulasPanel.Children.Add(item);
            }
        }

        private void InicializarTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (VideoPlayer == null || _arrastando) return;
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderVideo.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SliderVideo.Value = VideoPlayer.Position.TotalSeconds;
                TxtTempo.Text = Formatar(VideoPlayer.Position) + " / " +
                                      Formatar(VideoPlayer.NaturalDuration.TimeSpan);
            }
        }

        private string Formatar(TimeSpan t) =>
            t.TotalHours >= 1
                ? $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}"
                : $"{t.Minutes:D2}:{t.Seconds:D2}";

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer == null) return;
            if (_isPlaying) { VideoPlayer.Pause(); BtnPlayPause.Content = "▶"; }
            else { VideoPlayer.Play(); BtnPlayPause.Content = "⏸"; }
            _isPlaying = !_isPlaying;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer?.Stop();
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
        }

        private void BtnVoltar10_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer == null) return;
            VideoPlayer.Position = TimeSpan.FromSeconds(Math.Max(0, VideoPlayer.Position.TotalSeconds - 10));
        }

        private void BtnAvancar10_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer == null) return;
            VideoPlayer.Position = TimeSpan.FromSeconds(VideoPlayer.Position.TotalSeconds + 10);
        }

        private void SliderVideo_DragStarted(object sender, DragStartedEventArgs e)
            => _arrastando = true;

        private void SliderVideo_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (VideoPlayer != null)
                VideoPlayer.Position = TimeSpan.FromSeconds(SliderVideo.Value);
            _arrastando = false;
        }

        private void SliderVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer != null && _arrastando)
                VideoPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer != null) VideoPlayer.Volume = e.NewValue;
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
                SliderVideo.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;

            VideoPlayer.Volume = SliderVolume.Value;
            VideoPlayer.IsMuted = false;    
            OverlaySemVideo.Visibility = Visibility.Collapsed;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            BtnPlayPause.Content = "▶";

            try
            {
                using var ctx = new LearnixDbContext();
                var progSvc = new ProgressoService(ctx);
                progSvc.RegistrarConclusaoAula(_matriculaId, _aulaId);

                // Atualiza lista
                var concluidas = ctx.AulasConcluidas
                    .Where(ac => ac.MatriculaId == _matriculaId)
                    .Select(ac => ac.AulaId)
                    .ToHashSet();
                RenderizarListaAulas(concluidas);
            }
            catch { }
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
            => OverlaySemVideo.Visibility = Visibility.Visible;

        private void BtnAbrirVideo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Vídeos (*.mp4;*.wmv;*.avi)|*.mp4;*.wmv;*.avi|Todos (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    VideoPlayer.Source = new Uri(dlg.FileName, UriKind.Absolute);
                    OverlaySemVideo.Visibility = Visibility.Collapsed;
                    VideoPlayer.Play();
                    _isPlaying = true;
                    BtnPlayPause.Content = "⏸";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Falha ao abrir vídeo: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }

        private void AbaMateriaisClick(object sender, MouseButtonEventArgs e)
        {
            AbaMateriais.Visibility = Visibility.Visible;
            AbaTranscricao.Visibility = Visibility.Collapsed;
            AbaAnotacoes.Visibility = Visibility.Collapsed;
        }

        private void AbaTranscricaoClick(object sender, MouseButtonEventArgs e)
        {
            AbaMateriais.Visibility = Visibility.Collapsed;
            AbaTranscricao.Visibility = Visibility.Visible;
            AbaAnotacoes.Visibility = Visibility.Collapsed;
        }

        private void AbaAnotacoesClick(object sender, MouseButtonEventArgs e)
        {
            AbaMateriais.Visibility = Visibility.Collapsed;
            AbaTranscricao.Visibility = Visibility.Collapsed;
            AbaAnotacoes.Visibility = Visibility.Visible;
        }

        private void BtnSalvarAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Anotações salvas!", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AulaLista_Click(object sender, MouseButtonEventArgs e) { }
    }
}