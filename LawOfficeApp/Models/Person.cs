// ============ MODELS/PERSON.CS ============

using System;
using System.ComponentModel.DataAnnotations;

namespace LawOfficeApp.Models
{
    // Base class - demonstrates Inheritance
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Polymorphism - Virtual method
        public virtual string GetFullName() => $"{FirstName} {LastName}";
    }
}