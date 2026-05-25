using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Learnix.data;
using Learnix.model;
using Learnix.Services;
using Microsoft.Win32;

namespace Learnix
{
    public partial class TelaPlayer : UserControl
    {
        private string _nomeAluno = string.Empty;
        private string _nomeCurso = string.Empty;
        private bool _isPlaying = false;
        private bool _arrastando = false;
        private DispatcherTimer? _timer;
        private int _matriculaId;
        private int _aulaId;

        public TelaPlayer()
        {
            InitializeComponent();
            InicializarTimer();
            VolumePadrao();
        }

        private void VolumePadrao()
        {
            if (VideoPlayer != null) VideoPlayer.Volume = 0.7;
        }

        public void DefinirAula(Matricula matricula, Aula aula)
        {
            _matriculaId = matricula.Id;
            _aulaId = aula.Id;
            _nomeCurso = matricula.Curso?.Titulo ?? string.Empty;

            if (matricula.Aluno != null)
            {
                _nomeAluno = matricula.Aluno.Nome;
                Sidebar?.DefinirAluno(matricula.Aluno.Nome);
            }

            TxtTituloAula.Text = aula.Titulo ?? "Aula";
            TxtNomeCurso.Text = _nomeCurso;

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

        private void InicializarTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Nao atualiza o slider enquanto o usuario esta arrastando
            if (VideoPlayer == null || _arrastando) return;

            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderVideo.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SliderVideo.Value = VideoPlayer.Position.TotalSeconds;
                TxtTempo.Text = Formatar(VideoPlayer.Position) + " / " + Formatar(VideoPlayer.NaturalDuration.TimeSpan);
            }
        }

        private string Formatar(TimeSpan t)
        {
            return (t.TotalHours >= 1)
                ? string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds)
                : string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer == null) return;
            if (_isPlaying)
            {
                VideoPlayer.Pause();
                BtnPlayPause.Content = "Play";
            }
            else
            {
                VideoPlayer.Play();
                BtnPlayPause.Content = "Pause";
            }
            _isPlaying = !_isPlaying;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer?.Stop();
            _isPlaying = false;
            BtnPlayPause.Content = "Play";
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

        // ── Slider de progresso do video ─────────────────────────────────────

        /// <summary>
        /// Disparado quando o usuario COMECA a arrastar o slider.
        /// Seta _arrastando = true para pausar as atualizacoes do timer.
        /// Conectar no XAML: Thumb.DragStarted="SliderVideo_DragStarted"
        /// </summary>
        private void SliderVideo_DragStarted(object sender, DragStartedEventArgs e)
        {
            _arrastando = true;
        }

        /// <summary>
        /// Disparado quando o usuario SOLTA o slider.
        /// Aplica a nova posicao no video e libera o timer.
        /// Conectar no XAML: Thumb.DragCompleted="SliderVideo_DragCompleted"
        /// </summary>
        private void SliderVideo_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (VideoPlayer != null)
                VideoPlayer.Position = TimeSpan.FromSeconds(SliderVideo.Value);
            _arrastando = false;
        }

        /// <summary>
        /// Disparado a cada mudanca de valor do slider.
        /// So aplica a posicao enquanto o usuario esta arrastando (evita loop com o timer).
        /// </summary>
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

            OverlaySemVideo.Visibility = Visibility.Collapsed;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            BtnPlayPause.Content = "Play";

            // Marca aula como concluida via ProgressoService (idempotente)
            try
            {
                using var ctx = new LearnixDbContext();
                var progSvc = new ProgressoService(ctx);
                progSvc.RegistrarConclusaoAula(_matriculaId, _aulaId);
            }
            catch { /* silencioso — nao interrompe a UX */ }
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OverlaySemVideo.Visibility = Visibility.Visible;
        }

        private void BtnAbrirVideo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Videos (*.mp4;*.wmv;*.avi)|*.mp4;*.wmv;*.avi|Todos os arquivos (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    VideoPlayer.Source = new Uri(dlg.FileName, UriKind.Absolute);
                    OverlaySemVideo.Visibility = Visibility.Collapsed;
                    VideoPlayer.Play();
                    _isPlaying = true;
                    BtnPlayPause.Content = "Pause";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Falha ao abrir video: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos();
        }

        // ── Abas laterais ────────────────────────────────────────────────────

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
            MessageBox.Show("Anotacoes salvas!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AulaLista_Click(object sender, MouseButtonEventArgs e)
        {
            // Reservado para listagem dinamica de aulas no player.
        }
    }
}
