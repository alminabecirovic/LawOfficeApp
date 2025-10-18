using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.MVVM
{
    public class CasesViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;

        // Collections
        private ObservableCollection<Case> _cases;
        private ObservableCollection<object> _clientList;
        private ObservableCollection<object> _lawyerList;

        public ObservableCollection<Case> Cases
        {
            get => _cases;
            set => SetProperty(ref _cases, value);
        }

        public ObservableCollection<object> ClientList
        {
            get => _clientList;
            set => SetProperty(ref _clientList, value);
        }

        public ObservableCollection<object> LawyerList
        {
            get => _lawyerList;
            set => SetProperty(ref _lawyerList, value);
        }

        // Add Case Fields
        private string _caseTitle;
        private string _description;
        private object _selectedClient;
        private object _selectedLawyer;
        private DateTime? _deadline;

        public string CaseTitle
        {
            get => _caseTitle;
            set => SetProperty(ref _caseTitle, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public object SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value);
        }

        public object SelectedLawyer
        {
            get => _selectedLawyer;
            set => SetProperty(ref _selectedLawyer, value);
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set => SetProperty(ref _deadline, value);
        }

        // Case Details
        private Case _selectedCase;
        private string _caseDetailsInfo = "Izaberite predmet iznad za pregled detalja";

        public Case SelectedCase
        {
            get => _selectedCase;
            set
            {
                SetProperty(ref _selectedCase, value);
                LoadCaseDetails();
            }
        }

        public string CaseDetailsInfo
        {
            get => _caseDetailsInfo;
            set => SetProperty(ref _caseDetailsInfo, value);
        }

        // Commands
        public ICommand AddCaseCommand { get; }
        public ICommand SetActiveCommand { get; }
        public ICommand SetTrialCommand { get; }
        public ICommand SetResolvedCommand { get; }
        public ICommand SetRejectedCommand { get; }
        public ICommand SetOnHoldCommand { get; }

        public CasesViewModel(LawOfficeDbContext dbContext)
        {
            db = dbContext;

            AddCaseCommand = new RelayCommand(_ => AddCase());
            SetActiveCommand = new RelayCommand(_ => ChangeCaseStatus(CaseStatus.Active));
            SetTrialCommand = new RelayCommand(_ => ChangeCaseStatus(CaseStatus.Trial));
            SetResolvedCommand = new RelayCommand(_ => ChangeCaseStatus(CaseStatus.Resolved));
            SetRejectedCommand = new RelayCommand(_ => ChangeCaseStatus(CaseStatus.Rejected));
            SetOnHoldCommand = new RelayCommand(_ => ChangeCaseStatus(CaseStatus.OnHold));

            LoadData();
        }

        public void LoadData()
        {
            try
            {
                var cases = db.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .ToList();

                Cases = new ObservableCollection<Case>(cases);

                var clients = db.Clients.ToList();
                var lawyers = db.Lawyers.ToList();

                var clientsDisplay = clients.Select(c => new
                {
                    c.Id,
                    FullName = $"{c.FirstName} {c.LastName}"
                }).ToList();

                var lawyersDisplay = lawyers.Select(l => new
                {
                    l.Id,
                    FullName = $"{l.FirstName} {l.LastName} - {l.Specialization}"
                }).ToList();

                ClientList = new ObservableCollection<object>(clientsDisplay);
                LawyerList = new ObservableCollection<object>(lawyersDisplay);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cases: {ex.Message}", "Error");
            }
        }

        private void AddCase()
        {
            try
            {
                var clientData = SelectedClient as dynamic;
                var lawyerData = SelectedLawyer as dynamic;

                var caseItem = new Case
                {
                    CaseTitle = CaseTitle,
                    Description = Description,
                    ClientId = (int)clientData.Id,
                    LawyerId = (int)lawyerData.Id,
                    DeadlineDate = Deadline ?? DateTime.Now.AddDays(30),
                    Status = CaseStatus.Active
                };

                db.Cases.Add(caseItem);
                db.SaveChanges();

                CaseTitle = string.Empty;
                Description = string.Empty;

                LoadData();
                MessageBox.Show("Case added successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void LoadCaseDetails()
        {
            try
            {
                if (SelectedCase != null)
                {
                    var caseDetails = db.Cases
                        .Include(c => c.Client)
                        .Include(c => c.Lawyer)
                        .FirstOrDefault(c => c.Id == SelectedCase.Id);

                    if (caseDetails != null)
                    {
                        CaseDetailsInfo = $"Case: {caseDetails.CaseTitle}\n" +
                                          $"Client: {caseDetails.Client?.GetFullName()}\n" +
                                          $"Lawyer: {caseDetails.Lawyer?.GetFullName()}\n" +
                                          $"Status: {caseDetails.Status}\n" +
                                          $"Deadline: {caseDetails.DeadlineDate:d}\n" +
                                          $"Description: {caseDetails.Description}";
                    }
                }
                else
                {
                    CaseDetailsInfo = "Izaberite predmet iznad za pregled detalja";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading case details: {ex.Message}", "Error");
            }
        }

        private void ChangeCaseStatus(CaseStatus newStatus)
        {
            try
            {
                if (SelectedCase == null)
                {
                    MessageBox.Show("Please select a case first!", "Warning");
                    return;
                }

                var caseToUpdate = db.Cases.Find(SelectedCase.Id);
                if (caseToUpdate != null)
                {
                    caseToUpdate.Status = newStatus;
                    db.SaveChanges();

                    LoadData();
                    MessageBox.Show($"Case status changed to {newStatus}!", "Success");

                    var updatedCase = db.Cases
                        .Include(c => c.Client)
                        .Include(c => c.Lawyer)
                        .FirstOrDefault(c => c.Id == SelectedCase.Id);

                    if (updatedCase != null)
                    {
                        SelectedCase = updatedCase;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing status: {ex.Message}", "Error");
            }
        }
    }
}