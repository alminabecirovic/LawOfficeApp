// ============ MODELS/LAWYER.CS ============

using System.Collections.Generic;

namespace LawOfficeApp.Models
{
    // Lawyer class - Inherits from Person
    public class Lawyer : Person
    {
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public ICollection<Case> ActiveCases { get; set; } = new List<Case>();
        public decimal HourlyRate { get; set; }

        // Polymorphism - Override
        public override string GetFullName() => $"Atty. {base.GetFullName()} ({Specialization})";
    }
}