using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "Open";
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(30);
        public Client? Client { get; set; }
        public List<Document> Documents { get; set; } = new();

        public Case() { }

        public Case(string title, string status, DateTime deadline, Client client)
        {
            Title = title;
            Status = status;
            Deadline = deadline;
            Client = client;
        }
    }
}
