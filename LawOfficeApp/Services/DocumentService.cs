using System;
using System.Collections.Generic;
using System.Linq;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class DocumentService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;

        public DocumentService(LawOfficeDbContext context, EventMediator eventMediator)
        {
            _context = context;
            _eventMediator = eventMediator;
        }

        public List<Document> GetAllDocuments()
        {
            try
            {
                return _context.Documents
                    .Include(d => d.Case)
                    .ThenInclude(c => c.Client)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public Document GetDocumentById(int id)
        {
            try
            {
                return _context.Documents
                    .Include(d => d.Case)
                    .FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading document: {ex.Message}");
                return null;
            }
        }

        public bool AddDocument(Document document)
        {
            try
            {
                _context.Documents.Add(document);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Document {document.Title} added successfully");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error adding document: {ex.Message}");
                return false;
            }
        }

        public List<Document> GetDocumentsByCase(int caseId)
        {
            try
            {
                return _context.Documents
                    .Include(d => d.Case)
                    .Where(d => d.CaseId == caseId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public List<Document> SearchDocuments(string searchTerm)
        {
            try
            {
                return _context.Documents
                    .Include(d => d.Case)
                    .Where(d => d.Title.Contains(searchTerm) ||
                               d.FilePath.Contains(searchTerm))
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error searching documents: {ex.Message}");
                return new List<Document>();
            }
        }

        public bool DeleteDocument(int id)
        {
            try
            {
                var document = _context.Documents.Find(id);
                if (document == null)
                {
                    _eventMediator.RaiseDataChanged("Document not found");
                    return false;
                }

                _context.Documents.Remove(document);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Document {document.Title} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error deleting document: {ex.Message}");
                return false;
            }
        }

        public bool UpdateDocument(int id, string newTitle = null, string newFilePath = null)
        {
            try
            {
                var document = _context.Documents.Find(id);
                if (document == null)
                {
                    _eventMediator.RaiseDataChanged("Document not found");
                    return false;
                }

                if (!string.IsNullOrEmpty(newTitle))
                    document.Title = newTitle;

                if (!string.IsNullOrEmpty(newFilePath))
                    document.FilePath = newFilePath;

                _context.SaveChanges();

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