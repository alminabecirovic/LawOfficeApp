using System;
using System.ComponentModel.DataAnnotations;

namespace LawOfficeApp.Models
{
    // Document class
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string FilePath { get; set; }
        public int Importance { get; set; } // 1-5
        public int CaseId { get; set; }
        public Case Case { get; set; }
    }
}