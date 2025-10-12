using LawOfficeApp.Models;
using LawOfficeApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LawOfficeApp.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly LawOfficeService _service;

    public ObservableCollection<Advokat> Advokati { get; } = new();
    public ObservableCollection<Client> Klijenti { get; } = new();
    public ObservableCollection<Case> Slucajevi { get; } = new();

    private Advokat? _selectedAdvokat;
    public Advokat? SelectedAdvokat
    {
        get => _selectedAdvokat;
        set { _selectedAdvokat = value; OnPropertyChanged(); }
    }

    // Delegat + event
    public delegate void AdvokatAddedHandler(Advokat advokat);
    public event AdvokatAddedHandler? AdvokatAdded;

    public ICommand LoadCommand { get; }
    public ICommand AddAdvokatCommand { get; }
    public ICommand ShowUpcomingCommand { get; }

    public MainViewModel(LawOfficeService service)
    {
        _service = service;
        LoadCommand = new RelayCommand(async _ => await LoadAsync());
        AddAdvokatCommand = new RelayCommand(async _ => await AddAdvokatAsync());
        ShowUpcomingCommand = new RelayCommand(async _ => await ShowUpcomingDeadlinesAsync());
    }

    public async Task LoadAsync()
    {
        Advokati.Clear();
        Klijenti.Clear();
        Slucajevi.Clear();

        var adv = await _service.GetAllAdvokatiAsync();
        var kl = await _service.GetAllClientsAsync();
        var cs = await _service.GetAllCasesAsync();

        foreach (var a in adv) Advokati.Add(a);
        foreach (var k in kl) Klijenti.Add(k);
        foreach (var c in cs) Slucajevi.Add(c);
    }

    public async Task AddAdvokatAsync()
    {
        var newA = new Advokat("Nova", "Osoba", "Opšte pravo");
        await _service.AddAdvokatAsync(newA);
        Advokati.Add(newA);

        AdvokatAdded?.Invoke(newA); // event fired
        MessageBox.Show("Dodat novi advokat: " + newA.FullName);
    }

    public async Task ShowUpcomingDeadlinesAsync()
    {
        var upcoming = await _service.GetUpcomingDeadlinesAsync(7);
        if (upcoming.Any())
        {
            var list = string.Join("\n", upcoming.Select(c => $"{c.Title} - {c.Deadline:d} (klijent: {c.Client?.FullName})"));
            MessageBox.Show($"Rokovi u narednih 7 dana:\n{list}");
        }
        else
        {
            MessageBox.Show("Nema rokova u narednih 7 dana.");
        }
    }
}
