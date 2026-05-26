using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// Alias para nao conflitar com Learnix.model.Certificado
using CertModel = Learnix.model.Certificado;

namespace Learnix
{
    /// <summary>
    /// DTO de exibicao do certificado na UI.
    /// Separado do model para nao conflitar com Learnix.model.Certificado.
    /// </summary>
    public class CertificadoVM
    {
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

        // Acessa Sidebar diretamente (x:Name no XAML) em vez de FindName — mais simples e consistente
        public SidebarControl? SidebarNav => Sidebar;

        public TelaCertificados()
        {
            InitializeComponent();
            ListaCertificados.ItemsSource = _certificados;
            AtualizarEstado();
        }

        /// <summary>
        /// Define o aluno logado e carrega seus certificados reais do banco.
        /// </summary>
        public void DefinirAluno(Learnix.model.Aluno aluno)
        {
            _nomeAluno = aluno.Nome;
            Sidebar?.DefinirAluno(aluno.Nome);

            var certs = aluno.HistoricoMatriculas?
                .Where(m => m.Certificado != null)
                .Select(m => m.Certificado)
                .ToList() ?? new List<CertModel>();

            CarregarDoBanco(certs);
            AtualizarEstado();
        }

        // ── Carregar certificados reais vindos do banco ──────────────────────

        public void CarregarDoBanco(List<CertModel> certs)
        {
            _certificados.Clear();
            foreach (var c in certs)
            {
                _certificados.Add(new CertificadoVM
                {
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

        // ── Ver certificado ──────────────────────────────────────────────────

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

        // ── Imprimir / salvar como PDF ───────────────────────────────────────

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_certAtual == null) return;

            var dlg = new PrintDialog();
            if (dlg.ShowDialog() != true) return;

            dlg.PrintVisual(BorderCertificado, "Certificado — " + _certAtual.NomeCurso);

            MessageBox.Show(
                "Certificado enviado!\nDica: escolha 'Salvar como PDF' na impressora.",
                "Learnix", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    

        private void BtnBaixarPdf_Click(object sender, RoutedEventArgs e)
        {
            // Mesmo comportamento: imprimir / salvar como PDF
            BtnPdf_Click(sender, e);
        }
    }
}
