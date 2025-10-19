using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace LawOfficeApp.Models
{
    // Case Status Enumeration
    public enum CaseStatus
    {
        Active,
        Trial,
        Resolved,
        Rejected,
        OnHold
    }

    // Case class
    public class Case
    {
        [Key]
        public int Id { get; set; }
        public string CaseTitle { get; set; }
        public string Description { get; set; }
        public CaseStatus Status { get; set; } = CaseStatus.Active;
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public DateTime? ClosingDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int LawyerId { get; set; }
        public Lawyer Lawyer { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public decimal CostsSoFar { get; set; } = 0;
    }
}