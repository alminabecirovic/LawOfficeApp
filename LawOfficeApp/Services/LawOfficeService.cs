
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services;

public class LawOfficeService
{
    private readonly LawOfficeDbContext _context;
    private readonly Repository<Advokat> _advRepo;
    private readonly Repository<Client> _clientRepo;
    private readonly Repository<Case> _caseRepo;
    private readonly Repository<Invoice> _invoiceRepo;

    public LawOfficeService(LawOfficeDbContext context)
    {
        _context = context;
        _advRepo = new Repository<Advokat>(context);
        _clientRepo = new Repository<Client>(context);
        _caseRepo = new Repository<Case>(context);
        _invoiceRepo = new Repository<Invoice>(context);
    }

    // Advokat
    public Task<List<Advokat>> GetAllAdvokatiAsync() => _advRepo.GetAllAsync();
    public Task AddAdvokatAsync(Advokat adv) => _advRepo.AddAsync(adv);

    // Client
    public Task<List<Client>> GetAllClientsAsync() => _clientRepo.GetAllAsync();
    public Task AddClientAsync(Client c) => _clientRepo.AddAsync(c);

    // Case
    public Task<List<Case>> GetAllCasesAsync() => _caseRepo.GetAllAsync();
    public Task AddCaseAsync(Case c) => _caseRepo.AddAsync(c);

    // Invoice
    public Task<List<Invoice>> GetAllInvoicesAsync() => _invoiceRepo.GetAllAsync();
    public Task AddInvoiceAsync(Invoice i) => _invoiceRepo.AddAsync(i);
    public async Task MarkInvoicePaidAsync(int invoiceId)
    {
        var inv = await _invoiceRepo.GetByIdAsync(invoiceId);
        if (inv != null)
        {
            inv.Paid = true;
            await _invoiceRepo.UpdateAsync(inv);
        }
    }

    // LINQ example (server-side query)
    public async Task<List<Case>> GetUpcomingDeadlinesAsync(int daysAhead = 7)
    {
        var soon = DateTime.Now.AddDays(daysAhead);
        return await _context.Slucajevi
            .Include(c => c.Client)
            .Where(c => c.Deadline <= soon && c.Status == "Open")
            .OrderBy(c => c.Deadline)
            .ToListAsync();
    }
}
