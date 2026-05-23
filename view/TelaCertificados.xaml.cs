using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// Alias para não conflitar com Learnix.model.Certificado
using CertModel = Learnix.model.Certificado;

namespace Learnix
{
    /// <summary>
    /// DTO de exibição do certificado na UI.
    /// Separado do model para não conflitar com Learnix.model.Certificado.
    /// </summary>
    public class CertificadoVM
    {
        public string NomeCurso     { get; set; } = "";
        public string Professor     { get; set; } = "";
        public string CargaHoraria  { get; set; } = "";
        public string DataConclusao { get; set; } = "";
        public string Codigo        { get; set; } = "";
        public string NomeAluno     { get; set; } = "";
    }

    public partial class TelaCertificados : UserControl
    {
        private readonly ObservableCollection<CertificadoVM> _certificados = new();
        private CertificadoVM? _certAtual;
        private string _nomeAluno = "Aluno";

        // Lista estática — persiste durante toda a sessão do app
        public static List<CertificadoVM> CertificadosSessao { get; } = new();

        // Expõe a Sidebar para a MainWindow conectar os eventos de navegação
        public SidebarControl? SidebarNav => FindName("Sidebar") as SidebarControl;

        public TelaCertificados()
        {
            InitializeComponent();
            ListaCertificados.ItemsSource = _certificados;
            AtualizarEstado();
        }

        public void DefinirAluno(string nome)
        {
            _nomeAluno = nome;
            SidebarNav?.DefinirAluno(nome);
        }

        // ── Adicionar certificado via sessão (sem banco) ─────────────────────
        public static void AdicionarCertificado(
            string nomeAluno, string nomeCurso, string professor, string cargaHoraria)
        {
            CertificadosSessao.Add(new CertificadoVM
            {
                NomeAluno     = nomeAluno,
                NomeCurso     = nomeCurso,
                Professor     = professor,
                CargaHoraria  = cargaHoraria,
                DataConclusao = DateTime.Now.ToString("dd/MM/yyyy"),
                Codigo        = "LX-" + Guid.NewGuid().ToString("N")[..6].ToUpper()
            });
        }

        // ── Carregar certificados reais vindos do banco (TODO: chamar no login) ──
        public static void CarregarDoBanco(List<CertModel> certs)
        {
            CertificadosSessao.Clear();
            foreach (var c in certs)
            {
                CertificadosSessao.Add(new CertificadoVM
                {
                    NomeAluno     = c.Matricula?.Aluno?.Nome                      ?? "Aluno",
                    NomeCurso     = c.Matricula?.Curso?.Titulo                    ?? "Curso",
                    Professor     = c.Matricula?.Curso?.Instrutor?.Nome           ?? "Instrutor",
                    CargaHoraria  = (c.Matricula?.Curso?.CargaHoraria.ToString()  ?? "0") + "h",
                    DataConclusao = c.DataEmissao.ToString("dd/MM/yyyy"),
                    Codigo        = c.CodigoCertificado                           ?? "LX-000000"
                });
            }
        }

        private void AtualizarEstado()
        {
            _certificados.Clear();
            foreach (var c in CertificadosSessao)
                _certificados.Add(c);

            bool vazio = _certificados.Count == 0;
            PainelVazio.Visibility       = vazio ? Visibility.Visible  : Visibility.Collapsed;
            ListaCertificados.Visibility = vazio ? Visibility.Collapsed : Visibility.Visible;
            TxtTotalCerts.Text           = _certificados.Count.ToString();
        }

        // ── Ver certificado ──────────────────────────────────────────────────

        private void BtnVer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: CertificadoVM cert })
                MostrarCertificado(cert);
        }

        private void MostrarCertificado(CertificadoVM cert)
        {
            _certAtual            = cert;
            CertNomeAluno.Text    = cert.NomeAluno;
            CertNomeCurso.Text    = cert.NomeCurso;
            CertProfessor.Text    = cert.Professor;
            CertCargaHoraria.Text = "com carga horária de " + cert.CargaHoraria;
            CertData.Text         = cert.DataConclusao;
            CertCodigo.Text       = cert.Codigo;

            PainelLista.Visibility       = Visibility.Collapsed;
            PainelCertificado.Visibility = Visibility.Visible;
        }

        private void BtnVoltarLista_Click(object sender, MouseButtonEventArgs e)
        {
            PainelCertificado.Visibility = Visibility.Collapsed;
            PainelLista.Visibility       = Visibility.Visible;
            AtualizarEstado();
        }

        // ── Gerar PDF via PrintDialog ────────────────────────────────────────

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: CertificadoVM cert })
            {
                MostrarCertificado(cert);
                Dispatcher.BeginInvoke(GerarPdf,
                    System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void BtnBaixarPdf_Click(object sender, RoutedEventArgs e)
            => GerarPdf();

        private void GerarPdf()
        {
            if (_certAtual == null) return;

            var dlg = new PrintDialog();
            dlg.PrintTicket.PageOrientation = PageOrientation.Landscape;

            if (dlg.ShowDialog() != true) return;

            BorderCertificado.Measure(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
            BorderCertificado.Arrange(new Rect(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight)));
            dlg.PrintVisual(BorderCertificado, "Certificado — " + _certAtual.NomeCurso);

            MessageBox.Show(
                "Certificado enviado!\nDica: escolha 'Salvar como PDF' na impressora.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
