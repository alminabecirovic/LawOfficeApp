using System.Windows;
using LawOfficeApp.ViewModels;

namespace LawOfficeApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize main window with ViewModel
            MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();
        }
    }
}