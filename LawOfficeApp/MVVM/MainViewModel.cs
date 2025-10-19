using System.Windows;
using System.Windows.Input;
using LawOfficeApp.Data;
using LawOfficeApp.Services;

namespace LawOfficeApp.MVVM
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;
        private readonly EventMediator _eventMediator; //posrednik

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

        // Status Message Property
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Navigation Commands
        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowClientsCommand { get; }
        public ICommand ShowCasesCommand { get; }
        public ICommand ShowInvoicesCommand { get; }

        public MainViewModel()
        {
            db = new LawOfficeDbContext();

            //Event
            _eventMediator = new EventMediator();

            // Prosledjivanje eventa
            DashboardVM = new DashboardViewModel(db);
            ClientsVM = new ClientsViewModel(db);
            CasesVM = new CasesViewModel(db);
            InvoicesVM = new InvoicesViewModel(db);

            // Initialize Navigation Commands
            ShowDashboardCommand = new RelayCommand(_ => ShowTab("Dashboard"));
            ShowClientsCommand = new RelayCommand(_ => ShowTab("Clients"));
            ShowCasesCommand = new RelayCommand(_ => ShowTab("Cases"));
            ShowInvoicesCommand = new RelayCommand(_ => ShowTab("Invoices"));

            // osluskuj
            _eventMediator.DataChanged += OnDataChanged;
        }

        private void OnDataChanged(string message) //servis reaguje
        {
            StatusMessage = message;


            // Refreshuj podatke na trenutno aktivnom tabu
            if (DashboardVisibility == Visibility.Visible)
            {
                DashboardVM.LoadData();
            }
            else if (ClientsVisibility == Visibility.Visible)
            {
                ClientsVM.LoadData();
            }
            else if (CasesVisibility == Visibility.Visible)
            {
                CasesVM.LoadData();
            }
            else if (InvoicesVisibility == Visibility.Visible)
            {
                InvoicesVM.LoadData();
            }

            // Opciono: Prikaži MessageBox
            // MessageBox.Show(message, "Notification", MessageBoxButton.OK, 
            //     message.Contains("Error") ? MessageBoxImage.Error : MessageBoxImage.Information);
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

     
        public void Cleanup()
        {
            _eventMediator.DataChanged -= OnDataChanged;
        }
    }
}