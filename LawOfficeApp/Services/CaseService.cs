using System;
using System.Collections.Generic;
using System.Linq;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class CaseService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;

        public CaseService(LawOfficeDbContext context, EventMediator eventMediator)
        {
            _context = context;
            _eventMediator = eventMediator;
        }

        // Get all cases
        public List<Case> GetAllCases()
        {
            try
            {
                return _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Include(c => c.Documents)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading cases: {ex.Message}");
                return new List<Case>();
            }
        }

        // Get case by ID
        public Case GetCaseById(int id)
        {
            try
            {
                return _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Include(c => c.Documents)
                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading case: {ex.Message}");
                return null;
            }
        }

        // Add new case
        public bool AddCase(Case caseItem)
        {
            try
            {
                _context.Cases.Add(caseItem);
                _context.SaveChanges();

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
        public bool UpdateCaseStatus(int id, CaseStatus newStatus)
        {
            try
            {
                var caseItem = _context.Cases.Find(id);
                if (caseItem == null)
                {
                    _eventMediator.RaiseDataChanged("Case not found");
                    return false;
                }

                caseItem.Status = newStatus;
                _context.SaveChanges();

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
        public bool AssignLawyer(int caseId, int lawyerId)
        {
            try
            {
                var caseItem = _context.Cases.Find(caseId);
                var lawyer = _context.Lawyers.Find(lawyerId);

                if (caseItem == null || lawyer == null)
                {
                    _eventMediator.RaiseDataChanged("Case or Lawyer not found");
                    return false;
                }

                caseItem.LawyerId = lawyerId;
                _context.SaveChanges();

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
        public List<Case> GetCasesByStatus(CaseStatus status)
        {
            try
            {
                return _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Where(c => c.Status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading cases: {ex.Message}");
                return new List<Case>();
            }
        }

        // Get upcoming deadlines
        public List<Case> GetUpcomingDeadlines(int days = 30)
        {
            try
            {
                var deadline = DateTime.Now.AddDays(days);
                return _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Where(c => c.DeadlineDate <= deadline && c.DeadlineDate >= DateTime.Now)
                    .OrderBy(c => c.DeadlineDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading deadlines: {ex.Message}");
                return new List<Case>();
            }
        }

        // Delete case
        public bool DeleteCase(int id)
        {
            try
            {
                var caseItem = _context.Cases.Find(id);
                if (caseItem == null)
                {
                    _eventMediator.RaiseDataChanged("Case not found");
                    return false;
                }

                _context.Cases.Remove(caseItem);
                _context.SaveChanges();

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