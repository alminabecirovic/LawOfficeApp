using LawOfficeApp.Data;
using LawOfficeApp.Services;
using LawOfficeApp.ViewModels;
using System.Windows;

namespace LawOfficeApp.Views;

public partial class BillingWindow : Window
{
    public BillingWindow()
    {
        InitializeComponent();

        var ctx = new LawOfficeDbContext();
        var svc = new LawOfficeService(ctx);
        DataContext = new BillingViewModel(svc);
    }
}
