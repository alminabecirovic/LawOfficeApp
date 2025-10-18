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
    public class InvoicesViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;

        // Collections
        private ObservableCollection<Invoice> _invoices;
        private ObservableCollection<Case> _caseList;

        public ObservableCollection<Invoice> Invoices
        {
            get => _invoices;
            set => SetProperty(ref _invoices, value);
        }

        public ObservableCollection<Case> CaseList
        {
            get => _caseList;
            set => SetProperty(ref _caseList, value);
        }

        // Create Invoice Fields
        private string _invoiceNumber;
        private string _amount;
        private Case _selectedCase;

        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }

        public string Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public Case SelectedCase
        {
            get => _selectedCase;
            set => SetProperty(ref _selectedCase, value);
        }

        // Selected Invoice
        private Invoice _selectedInvoice;

        public Invoice SelectedInvoice
        {
            get => _selectedInvoice;
            set => SetProperty(ref _selectedInvoice, value);
        }

        // Commands
        public ICommand CreateInvoiceCommand { get; }
        public ICommand MarkAsPaidCommand { get; }
        public ICommand MarkAsUnpaidCommand { get; }

        public InvoicesViewModel(LawOfficeDbContext dbContext)
        {
            db = dbContext;

            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice());
            MarkAsPaidCommand = new RelayCommand(_ => ChangeInvoiceStatus(true));
            MarkAsUnpaidCommand = new RelayCommand(_ => ChangeInvoiceStatus(false));

            LoadData();
        }

        public void LoadData()
        {
            try
            {
                var invoices = db.Invoices.ToList();
                Invoices = new ObservableCollection<Invoice>(invoices);

                var cases = db.Cases.Include(c => c.Client).ToList();
                CaseList = new ObservableCollection<Case>(cases);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error");
            }
        }

        private void CreateInvoice()
        {
            try
            {
                var invoice = new Invoice
                {
                    InvoiceNumber = InvoiceNumber,
                    Amount = decimal.Parse(Amount),
                    CaseId = SelectedCase.Id,
                    IssueDate = DateTime.Now,
                    IsPaid = false
                };

                db.Invoices.Add(invoice);
                db.SaveChanges();

                InvoiceNumber = string.Empty;
                Amount = string.Empty;

                LoadData();
                MessageBox.Show("Invoice created successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void ChangeInvoiceStatus(bool isPaid)
        {
            try
            {
                if (SelectedInvoice == null)
                {
                    MessageBox.Show("Molim vas izaberite fakturu prvo!", "Upozorenje");
                    return;
                }

                var invoice = db.Invoices.Find(SelectedInvoice.Id);
                if (invoice != null)
                {
                    invoice.IsPaid = isPaid;
                    db.SaveChanges();

                    LoadData();
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