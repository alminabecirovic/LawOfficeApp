using System;
using System.Linq;
using System.Windows;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp
{
    public partial class MainWindow : Window
    {
        private LawOfficeDbContext db;
        private int? selectedCaseId = null;

        public MainWindow()
        {
            InitializeComponent();
            db = new LawOfficeDbContext();
            LoadAllData();
            LoadComboBoxes();
        }

        // NAVIGATION BUTTONS
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Dashboard");
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Clients");
        }

        private void BtnCases_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Cases");
        }

        private void BtnInvoices_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Invoices");
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            LoadComboBoxes();
            MessageBox.Show("Data refreshed!", "Success");
        }

        private void ShowTab(string tabName)
        {
            TabDashboard.Visibility = Visibility.Collapsed;
            TabClients.Visibility = Visibility.Collapsed;
            TabCases.Visibility = Visibility.Collapsed;
            TabInvoices.Visibility = Visibility.Collapsed;

            switch (tabName)
            {
                case "Dashboard":
                    TabDashboard.Visibility = Visibility.Visible;
                    break;
                case "Clients":
                    TabClients.Visibility = Visibility.Visible;
                    break;
                case "Cases":
                    TabCases.Visibility = Visibility.Visible;
                    break;
                case "Invoices":
                    TabInvoices.Visibility = Visibility.Visible;
                    break;
            }
        }

        // LOAD COMBOBOXES
        private void LoadComboBoxes()
        {
            try
            {
                var clients = db.Clients.ToList();
                var lawyers = db.Lawyers.ToList();
                var cases = db.Cases.Include(c => c.Client).ToList();

                // Kreiraj display liste sa FullName za prikaz
                var clientsDisplay = clients.Select(c => new
                {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email
                }).ToList();

                var lawyersDisplay = lawyers.Select(l => new
                {
                    Id = l.Id,
                    FullName = $"{l.FirstName} {l.LastName} - {l.Specialization}"
                }).ToList();

                // Clients tab - Update Client ComboBox
                CmbUpdateClient.ItemsSource = clientsDisplay;
                CmbUpdateClient.DisplayMemberPath = "FullName";
                CmbUpdateClient.SelectedValuePath = "Id";

                // Cases tab - Client ComboBox
                CmbCaseClient.ItemsSource = clientsDisplay;
                CmbCaseClient.DisplayMemberPath = "FullName";
                CmbCaseClient.SelectedValuePath = "Id";

                // Cases tab - Lawyer ComboBox
                CmbCaseLawyer.ItemsSource = lawyersDisplay;
                CmbCaseLawyer.DisplayMemberPath = "FullName";
                CmbCaseLawyer.SelectedValuePath = "Id";

                // Invoices tab - Case ComboBox
                CmbInvoiceCase.ItemsSource = cases;
                CmbInvoiceCase.DisplayMemberPath = "CaseTitle";
                CmbInvoiceCase.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading combo boxes: {ex.Message}", "Error");
            }
        }

        // LOAD DATA
        private void LoadAllData()
        {
            try
            {
                var cases = db.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .ToList();

                var clients = db.Clients
                    .Include(c => c.Cases)
                    .ToList();

                var documents = db.Documents.ToList();
                var invoices = db.Invoices.ToList();

                // Dashboard
                GridDashboardCases.ItemsSource = cases;
                GridDashboardDeadlines.ItemsSource = cases;

                // Clients
                GridClients.ItemsSource = clients;

                // Cases
                GridCases.ItemsSource = cases;

                // Invoices
                GridInvoices.ItemsSource = invoices;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error");
            }
        }

        // ADD CLIENT
        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new Client
                {
                    FirstName = TxtClientFirstName.Text,
                    LastName = TxtClientLastName.Text,
                    Email = TxtClientEmail.Text,
                    PhoneNumber = TxtClientPhone.Text
                };

                db.Clients.Add(client);
                db.SaveChanges();

                TxtClientFirstName.Clear();
                TxtClientLastName.Clear();
                TxtClientEmail.Clear();
                TxtClientPhone.Clear();

                LoadAllData();
                LoadComboBoxes();
                MessageBox.Show("Client added successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        // UPDATE CLIENT
        private void BtnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbUpdateClient.SelectedValue == null)
                {
                    MessageBox.Show("Please select a client!", "Error");
                    return;
                }

                int id = (int)CmbUpdateClient.SelectedValue;
                var client = db.Clients.Find(id);

                if (client != null)
                {
                    client.Email = TxtUpdateEmail.Text;
                    db.SaveChanges();

                    TxtUpdateEmail.Clear();

                    LoadAllData();
                    MessageBox.Show("Client updated successfully!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        // ADD CASE
        private void BtnAddCase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var caseItem = new Case
                {
                    CaseTitle = TxtCaseTitle.Text,
                    Description = TxtCaseDesc.Text,
                    ClientId = (int)CmbCaseClient.SelectedValue,
                    LawyerId = (int)CmbCaseLawyer.SelectedValue,
                    DeadlineDate = DateCaseDeadline.SelectedDate ?? DateTime.Now.AddDays(30),
                    Status = CaseStatus.Active
                };

                db.Cases.Add(caseItem);
                db.SaveChanges();

                TxtCaseTitle.Clear();
                TxtCaseDesc.Clear();

                LoadAllData();
                LoadComboBoxes();
                MessageBox.Show("Case added successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        // GRID CASES SELECTION CHANGED
        private void GridCases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (GridCases.SelectedItem is Case selectedCase)
                {
                    selectedCaseId = selectedCase.Id;

                    // Load case details
                    var caseDetails = db.Cases
                        .Include(c => c.Client)
                        .Include(c => c.Lawyer)
                        .FirstOrDefault(c => c.Id == selectedCase.Id);

                    if (caseDetails != null)
                    {
                        TxtCaseDetailsInfo.Text = $"Case: {caseDetails.CaseTitle}\n" +
                                                  $"Client: {caseDetails.Client?.GetFullName()}\n" +
                                                  $"Lawyer: {caseDetails.Lawyer?.GetFullName()}\n" +
                                                  $"Status: {caseDetails.Status}\n" +
                                                  $"Deadline: {caseDetails.DeadlineDate:d}\n" +
                                                  $"Description: {caseDetails.Description}";
                    }
                }
                else
                {
                    selectedCaseId = null;
                    TxtCaseDetailsInfo.Text = "Select a case above to view details";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading case details: {ex.Message}", "Error");
            }
        }

        // SET ACTIVE
        private void BtnSetActive_Click(object sender, RoutedEventArgs e)
        {
            ChangeCaseStatus(CaseStatus.Active);
        }

        // SET TRIAL
        private void BtnSetTrial_Click(object sender, RoutedEventArgs e)
        {
            ChangeCaseStatus(CaseStatus.Trial);
        }

        // SET RESOLVED
        private void BtnSetResolved_Click(object sender, RoutedEventArgs e)
        {
            ChangeCaseStatus(CaseStatus.Resolved);
        }

        // SET REJECTED
        private void BtnSetRejected_Click(object sender, RoutedEventArgs e)
        {
            ChangeCaseStatus(CaseStatus.Rejected);
        }

        // SET ON HOLD
        private void BtnSetOnHold_Click(object sender, RoutedEventArgs e)
        {
            ChangeCaseStatus(CaseStatus.OnHold);
        }

        // HELPER METHOD - CHANGE CASE STATUS
        private void ChangeCaseStatus(CaseStatus newStatus)
        {
            try
            {
                if (selectedCaseId == null)
                {
                    MessageBox.Show("Please select a case first!", "Warning");
                    return;
                }

                var caseToUpdate = db.Cases.Find(selectedCaseId);
                if (caseToUpdate != null)
                {
                    caseToUpdate.Status = newStatus;
                    db.SaveChanges();

                    LoadAllData();
                    MessageBox.Show($"Case status changed to {newStatus}!", "Success");

                    // Refresh detalje
                    var updatedCase = db.Cases
                        .Include(c => c.Client)
                        .Include(c => c.Lawyer)
                        .FirstOrDefault(c => c.Id == selectedCaseId);

                    if (updatedCase != null)
                    {
                        TxtCaseDetailsInfo.Text = $"Case: {updatedCase.CaseTitle}\n" +
                                                  $"Client: {updatedCase.Client?.GetFullName()}\n" +
                                                  $"Lawyer: {updatedCase.Lawyer?.GetFullName()}\n" +
                                                  $"Status: {updatedCase.Status}\n" +
                                                  $"Deadline: {updatedCase.DeadlineDate:d}\n" +
                                                  $"Description: {updatedCase.Description}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing status: {ex.Message}", "Error");
            }
        }

        // CREATE INVOICE
        private void BtnCreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var invoice = new Invoice
                {
                    InvoiceNumber = TxtInvoiceNumber.Text,
                    Amount = decimal.Parse(TxtInvoiceAmount.Text),
                    CaseId = (int)CmbInvoiceCase.SelectedValue,
                    IssueDate = DateTime.Now,
                    IsPaid = false
                };

                db.Invoices.Add(invoice);
                db.SaveChanges();

                TxtInvoiceNumber.Clear();
                TxtInvoiceAmount.Clear();

                LoadAllData();
                MessageBox.Show("Invoice created successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
        private int? selectedInvoiceId = null;

        // INVOICE SELECTION CHANGED
        private void GridInvoices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (GridInvoices.SelectedItem is Invoice selectedInvoice)
            {
                selectedInvoiceId = selectedInvoice.Id;
            }
            else
            {
                selectedInvoiceId = null;
            }
        }

        // MARK AS PAID
        private void BtnMarkAsPaid_Click(object sender, RoutedEventArgs e)
        {
            ChangeInvoiceStatus(true);
        }

        // MARK AS UNPAID
        private void BtnMarkAsUnpaid_Click(object sender, RoutedEventArgs e)
        {
            ChangeInvoiceStatus(false);
        }

        // HELPER METHOD - CHANGE INVOICE STATUS
        private void ChangeInvoiceStatus(bool isPaid)
        {
            try
            {
                if (selectedInvoiceId == null)
                {
                    MessageBox.Show("Molim vas izaberite fakturu prvo!", "Upozorenje");
                    return;
                }

                var invoice = db.Invoices.Find(selectedInvoiceId);
                if (invoice != null)
                {
                    invoice.IsPaid = isPaid;
                    db.SaveChanges();

                    LoadAllData();
                    string status = isPaid ? "plaćeno" : "neplaćeno";
                    MessageBox.Show($"Faktura označena kao {status}!", "Uspeh");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}", "Greška");
            }
        }
    }
}