
// ============ DATABASE CONTEXT - Entity Framework Core ============
// Configured for SQL Server

using Microsoft.EntityFrameworkCore;
using LawOfficeApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Windows;

namespace LawOfficeApp.Data
{
    public class LawOfficeDbContext : DbContext
    {
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // SQL Server connection - modify as needed for your local setup
            options.UseSqlServer(
                "Server = (localdb)\\MSSQLLocalDB; Database = LawOfficeDB; Trusted_Connection = True"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // Case to Lawyer (One-to-Many)
            modelBuilder.Entity<Case>()
                .HasOne(c => c.Lawyer)
                .WithMany(l => l.ActiveCases)
                .HasForeignKey(c => c.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Case to Client (One-to-Many)
            modelBuilder.Entity<Case>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Cases)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Document to Case (One-to-Many)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Case)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invoice to Case (One-to-Many)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Case)
                .WithMany()
                .HasForeignKey(i => i.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table per hierarchy for Person (inheritance)
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Lawyer>("Lawyer")
                .HasValue<Client>("Client");
        }
    }
}