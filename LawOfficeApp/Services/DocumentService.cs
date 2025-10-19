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
    public class DocumentService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;
        private readonly IRepository<Document> _documentRepository;

        public DocumentService(LawOfficeDbContext context, EventMediator eventMediator,
                             IRepository<Document> documentRepository)
        {
            _context = context;
            _eventMediator = eventMediator;
            _documentRepository = documentRepository;
        }

        public async Task<List<Document>> GetAllDocuments()
        {
            try
            {
                return await _context.Documents
                    .Include(d => d.Case)
                    .ThenInclude(c => c.Client)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public async Task<Document> GetDocumentById(int id)
        {
            try
            {
                return await _context.Documents
                    .Include(d => d.Case)
                    .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading document: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddDocument(Document document)
        {
            try
            {
                await _documentRepository.AddAsync(document);

                _eventMediator.RaiseDataChanged($"Document {document.Title} added successfully");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error adding document: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Document>> GetDocumentsByCase(int caseId)
        {
            try
            {
                return await _context.Documents
                    .Include(d => d.Case)
                    .Where(d => d.CaseId == caseId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public async Task<List<Document>> SearchDocuments(string searchTerm)
        {
            try
            {
                return await _context.Documents
                    .Include(d => d.Case)
                    .Where(d => d.Title.Contains(searchTerm) ||
                               d.FilePath.Contains(searchTerm))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error searching documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public async Task<bool> DeleteDocument(int id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    _eventMediator.RaiseDataChanged("Document not found");
                    return false;
                }

                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();

                _eventMediator.RaiseDataChanged($"Document {document.Title} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error deleting document: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDocument(int id, string newTitle = null, string newFilePath = null)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    _eventMediator.RaiseDataChanged("Document not found");
                    return false;
                }

                if (!string.IsNullOrEmpty(newTitle))
                    document.Title = newTitle;

                if (!string.IsNullOrEmpty(newFilePath))
                    document.FilePath = newFilePath;

                await _documentRepository.UpdateAsync(document);

                _eventMediator.RaiseDataChanged($"Document {document.Title} updated");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error updating document: {ex.Message}");
                return false;
            }
        }
    }
}