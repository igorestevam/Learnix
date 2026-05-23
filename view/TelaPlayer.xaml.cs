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
        private string _nomeAluno   = "Aluno";
        private string _nomeCurso   = "";
        private bool   _isPlaying   = false;
        private bool   _arrastando  = false;
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
            TxtNomeCurso.Text  = nomeCurso;
            _nomeCurso         = nomeCurso;
            _nomeAluno         = nomeAluno;
            Sidebar.DefinirAluno(nomeAluno);
        }

        // ── Timer para atualizar barra de progresso ──────────────────────────

        private void InicializarTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan && !_arrastando)
            {
                var total    = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                var atual    = VideoPlayer.Position.TotalSeconds;
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
                _timer.Start();
                OverlaySemVideo.Visibility = Visibility.Collapsed;
            }
            else
            {
                VideoPlayer.Pause();
                _isPlaying = false;
                BtnPlayPause.Content = "▶";
                _timer.Stop();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
            _timer.Stop();
            SliderVideo.Value = 0;
            TxtTempo.Text = "00:00 / 00:00";
        }

        private void BtnVoltar10_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Position -= TimeSpan.FromSeconds(10);
        }

        private void BtnAvancar10_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Position += TimeSpan.FromSeconds(10);
        }

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

        // ── Abrir arquivo de vídeo ───────────────────────────────────────────

        private void BtnAbrirVideo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title  = "Selecionar vídeo da aula",
                Filter = "Vídeos|*.mp4;*.avi;*.mkv;*.wmv;*.mov|Todos os arquivos|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                VideoPlayer.Source = new Uri(dlg.FileName);
                OverlaySemVideo.Visibility = Visibility.Collapsed;
                VideoPlayer.Play();
                _isPlaying = true;
                BtnPlayPause.Content = "⏸";
                _timer.Start();
            }
        }

        // ── Eventos do MediaElement ──────────────────────────────────────────

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
                TxtTempo.Text = $"00:00 / {Formatar(VideoPlayer.NaturalDuration.TimeSpan)}";
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            BtnPlayPause.Content = "▶";
            _timer.Stop();
            VideoPlayer.Position = TimeSpan.Zero;
            SliderVideo.Value = 0;
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Não foi possível carregar o vídeo.\n{e.ErrorException?.Message}",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Error);
            OverlaySemVideo.Visibility = Visibility.Visible;
        }

        // ── Abas ─────────────────────────────────────────────────────────────

        private void AbaMateriaisClick(object sender, MouseButtonEventArgs e)
            => MostrarAba("Materiais");

        private void AbaTranscricaoClick(object sender, MouseButtonEventArgs e)
            => MostrarAba("Transcricao");

        private void AbaAnotacoesClick(object sender, MouseButtonEventArgs e)
            => MostrarAba("Anotacoes");

        private void MostrarAba(string aba)
        {
            AbaMateriais.Visibility    = aba == "Materiais"  ? Visibility.Visible : Visibility.Collapsed;
            AbaTranscricao.Visibility  = aba == "Transcricao"? Visibility.Visible : Visibility.Collapsed;
            AbaAnotacoes.Visibility    = aba == "Anotacoes"  ? Visibility.Visible : Visibility.Collapsed;

            var ativo    = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7E6BAC"));
            var inativo  = new SolidColorBrush(Colors.Transparent);
            var branco   = new SolidColorBrush(Colors.White);
            var cinza    = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E8FC0"));

            AbaMateriaisHeader.BorderBrush    = aba == "Materiais"   ? ativo : inativo;
            AbaTranscricaoHeader.BorderBrush  = aba == "Transcricao" ? ativo : inativo;
            AbaAnotacoesHeader.BorderBrush    = aba == "Anotacoes"   ? ativo : inativo;

            (AbaMateriaisHeader.Child as TextBlock).Foreground   = aba == "Materiais"   ? branco : cinza;
            (AbaTranscricaoHeader.Child as TextBlock).Foreground = aba == "Transcricao" ? branco : cinza;
            (AbaAnotacoesHeader.Child as TextBlock).Foreground   = aba == "Anotacoes"   ? branco : cinza;
        }

        private void BtnSalvarAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            // TODO: persistir anotações no banco de dados
            MessageBox.Show("Anotações salvas!", "Learnix",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── Lista de aulas lateral ───────────────────────────────────────────

        private void AulaLista_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.Tag is string titulo)
            {
                TxtTituloAula.Text = titulo;
                BtnStop_Click(null, null);
                OverlaySemVideo.Visibility = Visibility.Visible;
                VideoPlayer.Source = null;
            }
        }

        // ── Navegação ────────────────────────────────────────────────────────

        private void BtnVoltar_Click(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            VideoPlayer.Stop();
            VideoPlayer.Source = null;

            var aulas = new TelaAulas();
            aulas.DefinirAluno(_nomeAluno);
            aulas.DefinirCurso(_nomeCurso, "", "", "", "", "68%");
            var main = Application.Current.MainWindow as MainWindow;
            main?.MostrarTela(aulas, _nomeAluno);
        }
    }
}
