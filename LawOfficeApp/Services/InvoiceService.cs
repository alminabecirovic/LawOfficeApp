using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using LawOfficeApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class InvoiceService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;
        private readonly IRepository<Invoice> _invoiceRepository;

        public InvoiceService(LawOfficeDbContext context, EventMediator eventMediator,
                            IRepository<Invoice> invoiceRepository)
        {
            _context = context;
            _eventMediator = eventMediator;
            _invoiceRepository = invoiceRepository;
        }

        // Get all invoices
        public async Task<List<Invoice>> GetAllInvoices()
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get invoice by ID
        public async Task<Invoice> GetInvoiceById(int id)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoice: {ex.Message}");
                return null;
            }
        }

        // Create new invoice
        public async Task<bool> CreateInvoice(Invoice invoice)
        {
            try
            {
                await _invoiceRepository.AddAsync(invoice);

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
        public async Task<bool> UpdatePaymentStatus(int id, bool isPaid)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    _eventMediator.RaiseDataChanged("Invoice not found");
                    return false;
                }

                invoice.IsPaid = isPaid;
                await _invoiceRepository.UpdateAsync(invoice);

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
        public async Task<List<Invoice>> GetUnpaidInvoices()
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .Where(i => !i.IsPaid)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading unpaid invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get paid invoices
        public async Task<List<Invoice>> GetPaidInvoices()
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.Case)
                    .ThenInclude(c => c.Client)
                    .Where(i => i.IsPaid)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading paid invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Get invoices by case
        public async Task<List<Invoice>> GetInvoicesByCase(int caseId)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.Case)
                    .Where(i => i.CaseId == caseId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        // Calculate total revenue
        public async Task<decimal> GetTotalRevenue(bool paidOnly = false)
        {
            try
            {
                var query = _context.Invoices.AsQueryable();

                if (paidOnly)
                    query = query.Where(i => i.IsPaid);

                return await query.SumAsync(i => i.Amount);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error calculating revenue: {ex.Message}");
                return 0;
            }
        }

        // Delete invoice
        public async Task<bool> DeleteInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    _eventMediator.RaiseDataChanged("Invoice not found");
                    return false;
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();

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