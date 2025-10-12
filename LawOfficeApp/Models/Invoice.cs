using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public Client? Client { get; set; }
        public Case? Case { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public bool Paid { get; set; } = false;

        public Invoice() { }

        public Invoice(Client client, Case slucaj, decimal amount)
        {
            Client = client;
            Case = slucaj;
            Amount = amount;
            Paid = false;
            IssueDate = DateTime.Now;
        }

        public override string ToString()
        {
            string status = Paid ? "Plaćeno" : "Neplaćeno";
            return $"{Client?.FullName} - {Case?.Title} - {Amount:C} ({status})";
        }
    }
}
