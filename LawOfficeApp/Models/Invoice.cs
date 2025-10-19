using System;
using System.ComponentModel.DataAnnotations;

namespace LawOfficeApp.Models
{
    // Invoice class - for billing
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
        public int CaseId { get; set; }
        public Case Case { get; set; }
    }
}