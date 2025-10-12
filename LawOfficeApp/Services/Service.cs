using System;
using LawOfficeApp.Models;

namespace LawOfficeApp.Services
{
    // Custom delegates for different operations
    public delegate void DataChangedDelegate(string message);
    public delegate void OperationCompletedDelegate(bool success, string message);
    public delegate void LawyerAssignedDelegate(Lawyer lawyer, Case caseItem);

    // EventArgs classes for custom events
    public class CaseEventArgs : EventArgs
    {
        public Case Case { get; set; }
        public string Action { get; set; } // Added, Updated, Deleted, StatusChanged
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class DocumentEventArgs : EventArgs
    {
        public Document Document { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class InvoiceEventArgs : EventArgs
    {
        public Invoice Invoice { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    // Event Mediator - centralized event management
    public class EventMediator
    {
        // Events using custom EventArgs
        public event EventHandler<CaseEventArgs> CaseChanged;
        public event EventHandler<DocumentEventArgs> DocumentChanged;
        public event EventHandler<InvoiceEventArgs> InvoiceChanged;

        // Events using delegates
        public event DataChangedDelegate DataChanged;
        public event OperationCompletedDelegate OperationCompleted;
        public event LawyerAssignedDelegate LawyerAssigned;

        // Raise case event
        public void RaiseCaseChanged(Case caseItem, string action)
        {
            CaseChanged?.Invoke(this, new CaseEventArgs
            {
                Case = caseItem,
                Action = action,
                Timestamp = DateTime.Now
            });
        }

        // Raise document event
        public void RaiseDocumentChanged(Document document, string action)
        {
            DocumentChanged?.Invoke(this, new DocumentEventArgs
            {
                Document = document,
                Action = action,
                Timestamp = DateTime.Now
            });
        }

        // Raise invoice event
        public void RaiseInvoiceChanged(Invoice invoice, string action)
        {
            InvoiceChanged?.Invoke(this, new InvoiceEventArgs
            {
                Invoice = invoice,
                Action = action,
                Timestamp = DateTime.Now
            });
        }

        // Raise data changed delegate event
        public void RaiseDataChanged(string message)
        {
            DataChanged?.Invoke(message);
        }

        // Raise operation completed delegate event
        public void RaiseOperationCompleted(bool success, string message)
        {
            OperationCompleted?.Invoke(success, message);
        }

        // Raise lawyer assigned delegate event
        public void RaiseLawyerAssigned(Lawyer lawyer, Case caseItem)
        {
            LawyerAssigned?.Invoke(lawyer, caseItem);
        }
    }
}
