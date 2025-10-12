// ============ MODELS/CLIENT.CS ============

using System.Collections.Generic;

namespace LawOfficeApp.Models
{
    // Client class - Inherits from Person
    public class Client : Person
    {
        public string Organization { get; set; }
        public ICollection<Case> Cases { get; set; } = new List<Case>();
        public string Notes { get; set; }

        public override string GetFullName() => $"Client: {base.GetFullName()}";
    }
}