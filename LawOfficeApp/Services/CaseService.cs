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
    public class CaseService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;
        private readonly IRepository<Case> _caseRepository;

        public CaseService(LawOfficeDbContext context, EventMediator eventMediator,
                          IRepository<Case> caseRepository)
        {
            _context = context;
            _eventMediator = eventMediator;
            _caseRepository = caseRepository;
        }

        // Get all cases
        public async Task<List<Case>> GetAllCases()
        {
            try
            {
                return await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Include(c => c.Documents)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading cases: {ex.Message}");
                return new List<Case>();
            }
        }

        // Get case by ID
        public async Task<Case> GetCaseById(int id)
        {
            try
            {
                return await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Include(c => c.Documents)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading case: {ex.Message}");
                return null;
            }
        }

        // Add new case
        public async Task<bool> AddCase(Case caseItem)
        {
            try
            {
                await _caseRepository.AddAsync(caseItem);

                _eventMediator.RaiseDataChanged($"Case {caseItem.CaseTitle} added successfully");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error adding case: {ex.Message}");
                return false;
            }
        }

        // Update case status
        public async Task<bool> UpdateCaseStatus(int id, CaseStatus newStatus)
        {
            try
            {
                var caseItem = await _context.Cases.FindAsync(id);
                if (caseItem == null)
                {
                    _eventMediator.RaiseDataChanged("Case not found");
                    return false;
                }

                caseItem.Status = newStatus;
                await _caseRepository.UpdateAsync(caseItem);

                _eventMediator.RaiseDataChanged($"Case status changed to {newStatus}");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error updating case status: {ex.Message}");
                return false;
            }
        }

        // Assign lawyer to case
        public async Task<bool> AssignLawyer(int caseId, int lawyerId)
        {
            try
            {
                var caseItem = await _context.Cases.FindAsync(caseId);
                var lawyer = await _context.Lawyers.FindAsync(lawyerId);

                if (caseItem == null || lawyer == null)
                {
                    _eventMediator.RaiseDataChanged("Case or Lawyer not found");
                    return false;
                }

                caseItem.LawyerId = lawyerId;
                await _caseRepository.UpdateAsync(caseItem);

                _eventMediator.RaiseDataChanged($"Lawyer {lawyer.GetFullName()} assigned to case");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error assigning lawyer: {ex.Message}");
                return false;
            }
        }

        // Get cases by status
        public async Task<List<Case>> GetCasesByStatus(CaseStatus status)
        {
            try
            {
                return await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Where(c => c.Status == status)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading cases: {ex.Message}");
                return new List<Case>();
            }
        }

        // Get upcoming deadlines
        public async Task<List<Case>> GetUpcomingDeadlines(int days = 30)
        {
            try
            {
                var deadline = DateTime.Now.AddDays(days);
                return await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Where(c => c.DeadlineDate <= deadline && c.DeadlineDate >= DateTime.Now)
                    .OrderBy(c => c.DeadlineDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading deadlines: {ex.Message}");
                return new List<Case>();
            }
        }

        // Delete case
        public async Task<bool> DeleteCase(int id)
        {
            try
            {
                var caseItem = await _context.Cases.FindAsync(id);
                if (caseItem == null)
                {
                    _eventMediator.RaiseDataChanged("Case not found");
                    return false;
                }

                _context.Cases.Remove(caseItem);
                await _context.SaveChangesAsync();

                _eventMediator.RaiseDataChanged($"Case {caseItem.CaseTitle} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error deleting case: {ex.Message}");
                return false;
            }
        }
    }
}