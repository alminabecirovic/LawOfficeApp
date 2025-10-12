using LawOfficeApp.Models;
using LawOfficeApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LawOfficeApp.ViewModels;

public class BillingViewModel : BaseViewModel
{
    private readonly LawOfficeService _service;
    public ObservableCollection<Invoice> Invoices { get; } = new();

    public ICommand LoadInvoicesCommand { get; }
    public ICommand AddInvoiceCommand { get; }
    public ICommand MarkPaidCommand { get; }

    public BillingViewModel(LawOfficeService service)
    {
        _service = service;
        LoadInvoicesCommand = new RelayCommand(async _ => await LoadAsync());
        AddInvoiceCommand = new RelayCommand(async _ => await AddSampleInvoiceAsync());
        MarkPaidCommand = new RelayCommand(async param => await MarkPaidAsync(param));
    }

    private async Task LoadAsync()
    {
        Invoices.Clear();
        var list = await _service.GetAllInvoicesAsync();
        foreach (var i in list) Invoices.Add(i);
    }

    private async Task AddSampleInvoiceAsync()
    {
        var clients = await _service.GetAllClientsAsync();
        var cases = await _service.GetAllCasesAsync();
        var client = clients.FirstOrDefault();
        var cs = cases.FirstOrDefault();
        if (client is not null && cs is not null)
        {
            var inv = new Invoice(client, cs, 1200m);
            await _service.AddInvoiceAsync(inv);
            Invoices.Add(inv);
        }
    }

    private async Task MarkPaidAsync(object? param)
    {
        if (param is Invoice inv)
        {
            await _service.MarkInvoicePaidAsync(inv.Id);
            inv.Paid = true;
            var idx = Invoices.IndexOf(inv);
            if (idx >= 0) Invoices[idx] = inv;
        }
    }
}
