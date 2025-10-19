using System;
using System.Collections.Generic;
using System.Linq;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class InvoiceService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;

        public InvoiceService(LawOfficeDbContext context, EventMediator eventMediator)
        {
            _context = context;
            _eventMediator = eventMediator;
        }

        // Get all invoices
        public List<Invoice> GetAllInvoices()
        {
            try
            {
                return _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get invoice by ID
        public Invoice GetInvoiceById(int id)
        {
            try
            {
                return _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .FirstOrDefault(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoice: {ex.Message}");
                return null;
            }
        }

        // Create new invoice
        public bool CreateInvoice(Invoice invoice)
        {
            try
            {
                _context.Invoices.Add(invoice);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Invoice {invoice.InvoiceNumber} created successfully");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error creating invoice: {ex.Message}");
                return false;
            }
        }

        // Update invoice payment status
        public bool UpdatePaymentStatus(int id, bool isPaid)
        {
            try
            {
                var invoice = _context.Invoices.Find(id);
                if (invoice == null)
                {
                    _eventMediator.RaiseDataChanged("Invoice not found");
                    return false;
                }

                invoice.IsPaid = isPaid;
                _context.SaveChanges();

                string status = isPaid ? "paid" : "unpaid";
                _eventMediator.RaiseDataChanged($"Invoice marked as {status}");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error updating invoice: {ex.Message}");
                return false;
            }
        }

        // Get unpaid invoices
        public List<Invoice> GetUnpaidInvoices()
        {
            try
            {
                return _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .Where(i => !i.IsPaid)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading unpaid invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get paid invoices
        public List<Invoice> GetPaidInvoices()
        {
            try
            {
                return _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .Where(i => i.IsPaid)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading paid invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get invoices by case
        public List<Invoice> GetInvoicesByCase(int caseId)
        {
            try
            {
                return _context.Invoices
                    .Include(i => i.Case)
                    .Where(i => i.CaseId == caseId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Calculate total revenue
        public decimal GetTotalRevenue(bool paidOnly = false)
        {
            try
            {
                var query = _context.Invoices.AsQueryable();

                if (paidOnly)
                    query = query.Where(i => i.IsPaid);

                return query.Sum(i => i.Amount);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error calculating revenue: {ex.Message}");
                return 0;
            }
        }

        // Delete invoice
        public bool DeleteInvoice(int id)
        {
            try
            {
                var invoice = _context.Invoices.Find(id);
                if (invoice == null)
                {
                    _eventMediator.RaiseDataChanged("Invoice not found");
                    return false;
                }

                _context.Invoices.Remove(invoice);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Invoice {invoice.InvoiceNumber} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error deleting invoice: {ex.Message}");
                return false;
            }
        }
    }
}