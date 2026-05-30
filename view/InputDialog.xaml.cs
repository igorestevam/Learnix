using System.Windows;
using System.Windows.Input;

namespace Learnix
{
    public partial class InputDialog : Window
    {
        public string Resposta { get; private set; } = "";

        public InputDialog(string titulo, string label)
        {
            InitializeComponent();
            Title = titulo;
            TxtLabel.Text = label;
            TxtInput.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Resposta = TxtInput.Text;
            DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnOk_Click(sender, e);
            if (e.Key == Key.Escape) BtnCancelar_Click(sender, e);
        }
    }
}