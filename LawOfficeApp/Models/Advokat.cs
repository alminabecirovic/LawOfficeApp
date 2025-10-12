using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Models
{
    public class Advokat : Person
    {
        public string Specialization { get; set; } = "";
        public List<Case> ActiveCases { get; set; } = new();

        public Advokat() : base() { }

        public Advokat(string firstName, string lastName, string specialization)
            : base(firstName, lastName)
        {
            Specialization = specialization;
        }
        public override string GetRole() => $"Advokat ({Specialization})";

        public override string ToString() => $"{FullName} — {Specialization}";

    }
}
