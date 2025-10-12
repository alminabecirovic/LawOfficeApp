using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Models
{
    public  class Document
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string Importance { get; set; } = "";
        public int CaseId { get; set; }

        public Document()
        {
            CreatedDate = DateTime.Now;
        }

        public Document(string type, string importance, int caseId)
        {
            Type = type;
            Importance = importance;
            CaseId = caseId;
            CreatedDate = DateTime.Now;
        }
    }
}
