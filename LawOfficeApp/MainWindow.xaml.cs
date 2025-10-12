using System.Windows;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using LawOfficeApp.ViewModels;
using LawOfficeApp.Models;
using System;

namespace LawOfficeApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdateViewVisibility();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentView")
            {
                UpdateViewVisibility();
            }
        }

        private void UpdateViewVisibility()
        {
            DashboardTab.Visibility = Visibility.Collapsed;
            ClientsTab.Visibility = Visibility.Collapsed;
            CasesTab.Visibility = Visibility.Collapsed;
            InvoicesTab.Visibility = Visibility.Collapsed;

            switch (viewModel.CurrentView)
            {
                case "Dashboard":
                    DashboardTab.Visibility = Visibility.Visible;
                    break;
                case "Clients":
                    ClientsTab.Visibility = Visibility.Visible;
                    break;
                case "Cases":
                    CasesTab.Visibility = Visibility.Visible;
                    break;
                case "Invoicing":
                    InvoicesTab.Visibility = Visibility.Visible;
                    break;
            }
        }

        private async void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new Client
                {
                    FirstName = TxtClientFirstName.Text,
                    LastName = TxtClientLastName.Text,
                    Email = TxtClientEmail.Text,
                    PhoneNumber = TxtClientPhone.Text,
                    Organization = TxtClientOrganization.Text
                };

                await viewModel.AddClientAsync(client);

                TxtClientFirstName.Clear();
                TxtClientLastName.Clear();
                TxtClientEmail.Clear();
                TxtClientPhone.Clear();
                TxtClientOrganization.Clear();

                MessageBox.Show("Client added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int clientId = int.Parse(TxtUpdateClientId.Text);
                var client = viewModel.Clients.FirstOrDefault(c => c.Id == clientId);

                if (client != null)
                {
                    if (!string.IsNullOrEmpty(TxtUpdateEmail.Text))
                        client.Email = TxtUpdateEmail.Text;
                    if (!string.IsNullOrEmpty(TxtUpdatePhone.Text))
                        client.PhoneNumber = TxtUpdatePhone.Text;

                    await viewModel.UpdateClientAsync(client);

                    TxtUpdateClientId.Clear();
                    TxtUpdateEmail.Clear();
                    TxtUpdatePhone.Clear();

                    MessageBox.Show("Client updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Client not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGridClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridClients.SelectedItem is Client selectedClient)
            {
                DataGridClientCases.ItemsSource = selectedClient.Cases;
            }
        }

        private async void BtnAddCase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var caseItem = new Case
                {
                    CaseTitle = TxtCaseTitle.Text,
                    Description = TxtCaseDesc.Text,
                    ClientId = (int)CmbCaseClient.SelectedValue,
                    LawyerId = (int)CmbCaseLawyer.SelectedValue,
                    DeadlineDate = TxtCaseDeadline.SelectedDate ?? DateTime.Now.AddDays(30),
                    Status = CaseStatus.Active
                };

                await viewModel.AddCaseAsync(caseItem);

                TxtCaseTitle.Clear();
                TxtCaseDesc.Clear();

                MessageBox.Show("Case added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGridCases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridCases.SelectedItem is Case selectedCase)
            {
                TxtCaseDetails.Text = $"Case: {selectedCase.CaseTitle}\n" +
                                      $"Description: {selectedCase.Description}\n" +
                                      $"Client: {selectedCase.Client?.GetFullName()}\n" +
                                      $"Lawyer: {selectedCase.Lawyer?.GetFullName()}\n" +
                                      $"Status: {selectedCase.Status}\n" +
                                      $"Deadline: {selectedCase.DeadlineDate:d}\n" +
                                      $"Opened: {selectedCase.OpeningDate:d}";

                DataGridCaseDocuments.ItemsSource = selectedCase.Documents;
            }
        }

        private async void BtnAddInvoice_Click(object sender, RoutedEventArgs e)
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

                await viewModel.AddInvoiceAsync(invoice);

                TxtInvoiceNumber.Clear();
                TxtInvoiceAmount.Clear();

                MessageBox.Show("Invoice created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}