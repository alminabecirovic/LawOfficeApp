using LawOfficeApp.Data;
using LawOfficeApp.Services;
using LawOfficeApp.ViewModels;
using System.Windows;

namespace LawOfficeApp.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();

        var ctx = new LawOfficeDbContext();
        DataSeeder.SeedIfEmpty(ctx); // seed demo data (optional)
        var service = new LawOfficeService(ctx);
        _vm = new MainViewModel(service);
        DataContext = _vm;

        _vm.AdvokatAdded += Vm_AdvokatAdded;
    }

    private void Vm_AdvokatAdded(Models.Advokat advokat)
    {
        MessageBox.Show($"Event: Dodan advokat {advokat.FullName}", "Obaveštenje");
    }

    private void OpenBilling_Click(object sender, RoutedEventArgs e)
    {
        var wnd = new BillingWindow();
        wnd.Owner = this;
        wnd.ShowDialog();
    }
}
