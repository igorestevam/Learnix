using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Learnix.data;

// Alias para evitar conflito de nomes
using CertModel = Learnix.model.Certificado;

namespace Learnix
{
    public class CertificadoVM
    {
        public int Id { get; set; }
        public string NomeCurso { get; set; } = "";
        public string Professor { get; set; } = "";
        public string CargaHoraria { get; set; } = "";
        public string DataConclusao { get; set; } = "";
        public string Codigo { get; set; } = "";
        public string NomeAluno { get; set; } = "";
    }

    public partial class TelaCertificados : UserControl
    {
        private readonly ObservableCollection<CertificadoVM> _certificados = new();
        private CertificadoVM? _certAtual;
        private string _nomeAluno = "Aluno";
        private int? _idCertificadoParaAbrir;

        public SidebarControl? SidebarNav => Sidebar;

        public TelaCertificados()
        {
            InitializeComponent();
            ListaCertificados.ItemsSource = _certificados;
            AtualizarEstado();
        }

        public TelaCertificados(int idCertificadoParaAbrir)
        {
            InitializeComponent();
            ListaCertificados.ItemsSource = _certificados;
            _idCertificadoParaAbrir = idCertificadoParaAbrir;
            AtualizarEstado();
        }

        public void DefinirAluno(Learnix.model.Aluno aluno)
        {
            _nomeAluno = aluno.Nome;
            Sidebar?.DefinirAluno(aluno.Nome);

            // CORREÇÃO: Buscamos do banco para garantir que Curso, Aluno e Instrutor venham preenchidos,
            // impedindo que a tentativa de imprimir um dado nulo trave o componente.
            using var db = new LearnixDbContext();
            var certs = db.Certificados
                .Include(c => c.Matricula).ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula).ThenInclude(m => m.Curso).ThenInclude(curso => curso.Instrutor)
                .Where(c => c.Matricula.AlunoId == aluno.Id)
                .ToList();

            CarregarDoBanco(certs);
            AtualizarEstado();

            if (_idCertificadoParaAbrir.HasValue)
            {
                var certParaAbrir = _certificados.FirstOrDefault(c => c.Id == _idCertificadoParaAbrir.Value);
                if (certParaAbrir != null)
                {
                    MostrarCertificado(certParaAbrir);
                }
                _idCertificadoParaAbrir = null;
            }
        }

        public void CarregarDoBanco(List<CertModel> certs)
        {
            _certificados.Clear();
            foreach (var c in certs)
            {
                _certificados.Add(new CertificadoVM
                {
                    Id = c.Id,
                    NomeAluno = c.Matricula?.Aluno?.Nome ?? "Aluno",
                    NomeCurso = c.Matricula?.Curso?.Titulo ?? "Curso",
                    Professor = c.Matricula?.Curso?.Instrutor?.Nome ?? "Instrutor",
                    CargaHoraria = (c.Matricula?.Curso?.CargaHoraria.ToString() ?? "0") + "h",
                    DataConclusao = c.DataEmissao.ToString("dd/MM/yyyy"),
                    Codigo = c.CodigoCertificado ?? "LX-000000"
                });
            }
        }

        private void AtualizarEstado()
        {
            bool vazio = _certificados.Count == 0;
            PainelVazio.Visibility = vazio ? Visibility.Visible : Visibility.Collapsed;
            ListaCertificados.Visibility = vazio ? Visibility.Collapsed : Visibility.Visible;
            TxtTotalCerts.Text = _certificados.Count.ToString();
        }

        private void BtnVer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: CertificadoVM cert })
                MostrarCertificado(cert);
        }

        private void MostrarCertificado(CertificadoVM cert)
        {
            _certAtual = cert;
            CertNomeAluno.Text = cert.NomeAluno;
            CertNomeCurso.Text = cert.NomeCurso;
            CertProfessor.Text = cert.Professor;
            CertCargaHoraria.Text = "com carga horaria de " + cert.CargaHoraria;
            CertData.Text = cert.DataConclusao;
            CertCodigo.Text = cert.Codigo;

            PainelLista.Visibility = Visibility.Collapsed;
            PainelCertificado.Visibility = Visibility.Visible;
        }

        private void BtnVoltarLista_Click(object sender, MouseButtonEventArgs e)
        {
            PainelCertificado.Visibility = Visibility.Collapsed;
            PainelLista.Visibility = Visibility.Visible;
        }

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_certAtual == null) return;

            try
            {
                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Imagem PNG (*.png)|*.png",
                    FileName = $"Certificado_{_certAtual.NomeCurso.Replace(" ", "_")}.png",
                    Title = "Salvar Certificado"
                };

                if (sfd.ShowDialog() == true)
                {
                    // 1. Pega o tamanho real que o certificado ocupa na sua interface
                    double width = BorderCertificado.ActualWidth;
                    double height = BorderCertificado.ActualHeight;

                    // 2. CORREÇÃO DE DPI: Verifica se o seu Windows está com zoom (125%, 150%...)
                    double dpiX = 96d;
                    double dpiY = 96d;
                    PresentationSource source = PresentationSource.FromVisual(BorderCertificado);
                    if (source != null && source.CompositionTarget != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    // 3. Calcula a quantidade exata de pixels físicos que a imagem precisa ter para não cortar
                    int pixelWidth = (int)(width * (dpiX / 96.0));
                    int pixelHeight = (int)(height * (dpiY / 96.0));

                    // 4. Cria um "Molde Vetorial" isolado, ignorando se o certificado está espremido na tela
                    System.Windows.Media.DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();
                    using (System.Windows.Media.DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        System.Windows.Media.VisualBrush brush = new System.Windows.Media.VisualBrush(BorderCertificado);
                        drawingContext.DrawRectangle(brush, null, new Rect(0, 0, width, height));
                    }

                    // 5. Renderiza a imagem perfeita aplicando a correção matemática do DPI
                    var renderBitmap = new System.Windows.Media.Imaging.RenderTargetBitmap(
                        pixelWidth, pixelHeight, dpiX, dpiY, System.Windows.Media.PixelFormats.Pbgra32);

                    renderBitmap.Render(drawingVisual);

                    // 6. Converte os pixels para um arquivo PNG e salva
                    var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(renderBitmap));

                    using (var fileStream = System.IO.File.Create(sfd.FileName))
                    {
                        encoder.Save(fileStream);
                    }

                    MessageBox.Show("Certificado salvo com sucesso!\nVocê já pode compartilhá-lo nas redes.", "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao gerar a imagem: {ex.Message}", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}