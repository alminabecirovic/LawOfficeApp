using System.Windows;
using System.Windows.Input;
using LawOfficeApp.Data;
using LawOfficeApp.MVVM.ViewModels;

namespace LawOfficeApp.MVVM
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;

        // Child ViewModels
        public DashboardViewModel DashboardVM { get; }
        public ClientsViewModel ClientsVM { get; }
        public CasesViewModel CasesVM { get; }
        public InvoicesViewModel InvoicesVM { get; }

        // Visibility Properties
        private Visibility _dashboardVisibility = Visibility.Visible;
        private Visibility _clientsVisibility = Visibility.Collapsed;
        private Visibility _casesVisibility = Visibility.Collapsed;
        private Visibility _invoicesVisibility = Visibility.Collapsed;

        public Visibility DashboardVisibility
        {
            get => _dashboardVisibility;
            set => SetProperty(ref _dashboardVisibility, value);
        }

        public Visibility ClientsVisibility
        {
            get => _clientsVisibility;
            set => SetProperty(ref _clientsVisibility, value);
        }

        public Visibility CasesVisibility
        {
            get => _casesVisibility;
            set => SetProperty(ref _casesVisibility, value);
        }

        public Visibility InvoicesVisibility
        {
            get => _invoicesVisibility;
            set => SetProperty(ref _invoicesVisibility, value);
        }

        // Navigation Commands
        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowClientsCommand { get; }
        public ICommand ShowCasesCommand { get; }
        public ICommand ShowInvoicesCommand { get; }

        public MainViewModel()
        {
            db = new LawOfficeDbContext();

            // Initialize Child ViewModels
            DashboardVM = new DashboardViewModel(db);
            ClientsVM = new ClientsViewModel(db);
            CasesVM = new CasesViewModel(db);
            InvoicesVM = new InvoicesViewModel(db);

            // Initialize Navigation Commands
            ShowDashboardCommand = new RelayCommand(_ => ShowTab("Dashboard"));
            ShowClientsCommand = new RelayCommand(_ => ShowTab("Clients"));
            ShowCasesCommand = new RelayCommand(_ => ShowTab("Cases"));
            ShowInvoicesCommand = new RelayCommand(_ => ShowTab("Invoices"));
        }

        private void ShowTab(string tabName)
        {
            DashboardVisibility = Visibility.Collapsed;
            ClientsVisibility = Visibility.Collapsed;
            CasesVisibility = Visibility.Collapsed;
            InvoicesVisibility = Visibility.Collapsed;

            switch (tabName)
            {
                case "Dashboard":
                    DashboardVisibility = Visibility.Visible;
                    DashboardVM.LoadData();
                    break;
                case "Clients":
                    ClientsVisibility = Visibility.Visible;
                    ClientsVM.LoadData();
                    break;
                case "Cases":
                    CasesVisibility = Visibility.Visible;
                    CasesVM.LoadData();
                    break;
                case "Invoices":
                    InvoicesVisibility = Visibility.Visible;
                    InvoicesVM.LoadData();
                    break;
            }
        }
    }
}