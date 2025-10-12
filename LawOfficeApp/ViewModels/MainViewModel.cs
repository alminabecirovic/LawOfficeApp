// ============ MAIN VIEW MODEL ============
// REQUIREMENTS MET: LINQ, Async/Await, Events, MVVM

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using LawOfficeApp.MVVM;
using LawOfficeApp.Services;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext dbContext;
        private readonly EventMediator eventMediator;

        // Observable collections for UI binding
        private ObservableCollection<Lawyer> lawyers;
        private ObservableCollection<Client> clients;
        private ObservableCollection<Case> cases;
        private ObservableCollection<Invoice> invoices;
        private ObservableCollection<Document> documents;

        private string currentView = "Dashboard";
        private string statusMessage = "";
        private int totalCases = 0;
        private int activeCases = 0;
        private int resolvedCases = 0;

        public ObservableCollection<Lawyer> Lawyers
        {
            get => lawyers;
            set => SetProperty(ref lawyers, value);
        }

        public ObservableCollection<Client> Clients
        {
            get => clients;
            set => SetProperty(ref clients, value);
        }

        public ObservableCollection<Case> Cases
        {
            get => cases;
            set => SetProperty(ref cases, value);
        }

        public ObservableCollection<Invoice> Invoices
        {
            get => invoices;
            set => SetProperty(ref invoices, value);
        }

        public ObservableCollection<Document> Documents
        {
            get => documents;
            set => SetProperty(ref documents, value);
        }

        public string CurrentView
        {
            get => currentView;
            set => SetProperty(ref currentView, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public int TotalCases
        {
            get => totalCases;
            set => SetProperty(ref totalCases, value);
        }

        public int ActiveCases
        {
            get => activeCases;
            set => SetProperty(ref activeCases, value);
        }

        public int ResolvedCases
        {
            get => resolvedCases;
            set => SetProperty(ref resolvedCases, value);
        }

        // Commands
        public ICommand LoadDataCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            dbContext = new LawOfficeDbContext();
            eventMediator = new EventMediator();

            // Initialize collections
            Lawyers = new ObservableCollection<Lawyer>();
            Clients = new ObservableCollection<Client>();
            Cases = new ObservableCollection<Case>();
            Invoices = new ObservableCollection<Invoice>();
            Documents = new ObservableCollection<Document>();

            // Initialize commands
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            NavigateCommand = new RelayCommand(p => CurrentView = p as string);
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());

            // Subscribe to events
            SubscribeToEvents();

            // Load data on initialization
            _ = InitializeAsync();
        }

        private void SubscribeToEvents()
        {
            eventMediator.CaseChanged += (s, e) =>
            {
                StatusMessage = $"Case '{e.Case.CaseTitle}' was {e.Action.ToLower()}";
                System.Diagnostics.Debug.WriteLine($"[EVENT] {StatusMessage} at {e.Timestamp}");
            };

            eventMediator.DocumentChanged += (s, e) =>
            {
                StatusMessage = $"Document '{e.Document.Title}' was {e.Action.ToLower()}";
            };

            eventMediator.DataChanged += (msg) =>
            {
                System.Diagnostics.Debug.WriteLine($"[DATA CHANGED] {msg}");
            };
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Load data
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        // REQUIREMENT: ASYNC/AWAIT - Asynchronous data loading
        public async Task LoadDataAsync()
        {
            try
            {
                // REQUIREMENT: LINQ - Query using LINQ
                var lawyersList = await dbContext.Lawyers
                    .Include(l => l.ActiveCases)
                    .AsNoTracking()
                    .ToListAsync();

                var clientsList = await dbContext.Clients
                    .Include(c => c.Cases)
                    .AsNoTracking()
                    .ToListAsync();

                var casesList = await dbContext.Cases
                    .Include(c => c.Lawyer)
                    .Include(c => c.Client)
                    .Include(c => c.Documents)
                    .AsNoTracking()
                    .ToListAsync();

                var invoicesList = await dbContext.Invoices
                    .Include(i => i.Case)
                    .AsNoTracking()
                    .ToListAsync();

                var documentsList = await dbContext.Documents
                    .AsNoTracking()
                    .ToListAsync();

                // Update collections
                UpdateCollection(Lawyers, lawyersList);
                UpdateCollection(Clients, clientsList);
                UpdateCollection(Cases, casesList);
                UpdateCollection(Invoices, invoicesList);
                UpdateCollection(Documents, documentsList);

                // LINQ: Calculate statistics
                TotalCases = casesList.Count;
                ActiveCases = casesList.Count(c => c.Status == CaseStatus.Active);
                ResolvedCases = casesList.Count(c => c.Status == CaseStatus.Resolved);

                StatusMessage = "Data loaded successfully";
                eventMediator.RaiseDataChanged("All data refreshed from database");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
            }
        }

        // Helper method to update ObservableCollection
        private void UpdateCollection<T>(ObservableCollection<T> collection, List<T> newItems)
        {
            collection.Clear();
            foreach (var item in newItems)
                collection.Add(item);
        }

        // Add Lawyer - Async operation with event
        public async Task AddLawyerAsync(Lawyer lawyer)
        {
            try
            {
                dbContext.Lawyers.Add(lawyer);
                await dbContext.SaveChangesAsync();
                Lawyers.Add(lawyer);
                eventMediator.RaiseDataChanged($"Lawyer {lawyer.GetFullName()} added successfully");
                eventMediator.RaiseOperationCompleted(true, "Lawyer added");
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // Add Client - Async operation with event
        public async Task AddClientAsync(Client client)
        {
            try
            {
                dbContext.Clients.Add(client);
                await dbContext.SaveChangesAsync();
                Clients.Add(client);
                eventMediator.RaiseDataChanged($"Client {client.GetFullName()} added successfully");
                eventMediator.RaiseOperationCompleted(true, "Client added");
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // Update Client - New method
        public async Task UpdateClientAsync(Client client)
        {
            try
            {
                dbContext.Clients.Update(client);
                await dbContext.SaveChangesAsync();
                eventMediator.RaiseDataChanged($"Client {client.GetFullName()} updated successfully");
                await LoadDataAsync(); // Refresh
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // Add Case - Async operation with event
        public async Task AddCaseAsync(Case caseItem)
        {
            try
            {
                dbContext.Cases.Add(caseItem);
                await dbContext.SaveChangesAsync();
                Cases.Add(caseItem);
                eventMediator.RaiseCaseChanged(caseItem, "Added");
                eventMediator.RaiseLawyerAssigned(caseItem.Lawyer, caseItem);
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // Add Document - Async operation with event
        public async Task AddDocumentAsync(Document document)
        {
            try
            {
                dbContext.Documents.Add(document);
                await dbContext.SaveChangesAsync();
                Documents.Add(document);
                eventMediator.RaiseDocumentChanged(document, "Added");
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // Add Invoice - Async operation with event
        public async Task AddInvoiceAsync(Invoice invoice)
        {
            try
            {
                dbContext.Invoices.Add(invoice);
                await dbContext.SaveChangesAsync();
                Invoices.Add(invoice);
                eventMediator.RaiseInvoiceChanged(invoice, "Added");
            }
            catch (Exception ex)
            {
                eventMediator.RaiseOperationCompleted(false, $"Error: {ex.Message}");
            }
        }

        // LINQ example: Get active cases by lawyer
        public List<Case> GetActiveCasesByLawyer(int lawyerId)
        {
            return Cases
                .Where(c => c.LawyerId == lawyerId && c.Status == CaseStatus.Active)
                .OrderByDescending(c => c.OpeningDate)
                .ToList();
        }

        // LINQ example: Get overdue cases
        public List<Case> GetOverdueCases()
        {
            return Cases
                .Where(c => c.Status == CaseStatus.Active && c.DeadlineDate < DateTime.Now)
                .OrderBy(c => c.DeadlineDate)
                .ToList();
        }

        // LINQ example: Get unpaid invoices
        public List<Invoice> GetUnpaidInvoices()
        {
            return Invoices
                .Where(i => !i.IsPaid)
                .OrderByDescending(i => i.IssueDate)
                .ToList();
        }

        // Cleanup
        public void Dispose()
        {
            dbContext?.Dispose();
        }
    }
}