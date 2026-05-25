using System.Windows;
using Learnix.data;

namespace Learnix
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Garante que o banco existe e popula dados iniciais (idempotente)
            try
            {
                using (var ctx = new LearnixDbContext())
                {
                    LearnixDbInitializer.Seed(ctx);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Erro ao inicializar o banco de dados:\n" + ex.Message,
                    "Learnix", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
