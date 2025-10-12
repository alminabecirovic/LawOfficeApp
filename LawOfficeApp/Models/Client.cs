using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Models
{
    public class Client : Person
    {
        public string ContactInfo { get; set; } = "";
        public Client() : base() { }
        public Client(string firstName, string lastName, string contactInfo)
                : base(firstName, lastName)
        {
            ContactInfo = contactInfo;
        }

        public override string GetRole() => "Klijent";
    }
}
