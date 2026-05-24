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
        private DispatcherTimer? _timer;
        private int _matriculaId;
        private int _aulaId;

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

        // ── Timer para atualizar barra de progresso ──────────────────────────

        private void InicializarTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan && !_arrastando)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                var atual = VideoPlayer.Position.TotalSeconds;
                SliderVideo.Value = total > 0 ? (atual / total) * 100 : 0;

                TxtTempo.Text = $"{Formatar(VideoPlayer.Position)} / " +
                    $"{Formatar(VideoPlayer.NaturalDuration.TimeSpan)}";
            }
        }

        private string Formatar(TimeSpan t)
            => t.TotalHours >= 1
                ? $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}"
                : $"{t.Minutes:D2}:{t.Seconds:D2}";

        // ── Controles do player ──────────────────────────────────────────────

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPlaying)
            {
                VideoPlayer.Play();
                _isPlaying = true;
                BtnPlayPause.Content = "⏸";
                _timer?.Start();
                OverlaySemVideo.Visibility = Visibility.Collapsed;
            }
            else
            {
                VideoPlayer.Pause();
                _isPlaying = false;
                BtnPlayPause.Content = "▶";
                _timer?.Stop();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
            _timer?.Stop();
            SliderVideo.Value = 0;
            TxtTempo.Text = "00:00 / 00:00";
        }

        private void BtnVoltar10_Click(object sender, RoutedEventArgs e)
            => VideoPlayer.Position -= TimeSpan.FromSeconds(10);

        private void BtnAvancar10_Click(object sender, RoutedEventArgs e)
            => VideoPlayer.Position += TimeSpan.FromSeconds(10);

        private void SliderVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_arrastando && VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                VideoPlayer.Position = TimeSpan.FromSeconds((e.NewValue / 100) * total);
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer != null)
                VideoPlayer.Volume = e.NewValue;
        }

        // ── Eventos do MediaElement ──────────────────────────────────────────

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            OverlaySemVideo.Visibility = Visibility.Collapsed;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
            _timer?.Stop();

            var controller = new AulaController(new ProgressoService(new LearnixDbContext()));
            controller.ConcluirAula(_matriculaId, _aulaId);
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Não foi possível carregar o vídeo: " + e.ErrorException?.Message,
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            OverlaySemVideo.Visibility = Visibility.Visible;
        }

        // ── Abrir arquivo de vídeo (fallback manual) ─────────────────────────

        private void BtnAbrirVideo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Vídeos|*.mp4;*.avi;*.mkv;*.wmv;*.mov|Todos os arquivos|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                VideoPlayer.Source = new Uri(dlg.FileName, UriKind.Absolute);
                OverlaySemVideo.Visibility = Visibility.Collapsed;
            }
        }

        // ── Voltar para tela de aulas ────────────────────────────────────────

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarMeusCursos(_nomeAluno);
        }

        // ── Abas: Materiais / Transcrição / Anotações ────────────────────────

        private void AbaMateriaisClick(object sender, MouseButtonEventArgs e)
        {
            if (AbaMateriais != null) AbaMateriais.Visibility = Visibility.Visible;
            if (AbaTranscricao != null) AbaTranscricao.Visibility = Visibility.Collapsed;
            if (AbaAnotacoes != null) AbaAnotacoes.Visibility = Visibility.Collapsed;
        }

        private void AbaTranscricaoClick(object sender, MouseButtonEventArgs e)
        {
            if (AbaMateriais != null) AbaMateriais.Visibility = Visibility.Collapsed;
            if (AbaTranscricao != null) AbaTranscricao.Visibility = Visibility.Visible;
            if (AbaAnotacoes != null) AbaAnotacoes.Visibility = Visibility.Collapsed;
        }

        private void AbaAnotacoesClick(object sender, MouseButtonEventArgs e)
        {
            if (AbaMateriais != null) AbaMateriais.Visibility = Visibility.Collapsed;
            if (AbaTranscricao != null) AbaTranscricao.Visibility = Visibility.Collapsed;
            if (AbaAnotacoes != null) AbaAnotacoes.Visibility = Visibility.Visible;
        }

        private void BtnSalvarAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Anotações salvas!",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── Lista de aulas lateral ────────────────────────────────────────────

        private void AulaLista_Click(object sender, MouseButtonEventArgs e)
        {
            // Navegação entre aulas pela lista lateral — implementar conforme estrutura do XAML
        }
    }
}
