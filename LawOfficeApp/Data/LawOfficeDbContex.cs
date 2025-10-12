using LawOfficeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace LawOfficeApp.Data;

public class LawOfficeDbContext : DbContext
{
    public DbSet<Advokat> Advokati { get; set; }
    public DbSet<Client> Klijenti { get; set; }
    public DbSet<Case> Slucajevi { get; set; }
    public DbSet<Document> Dokumenti { get; set; }
    public DbSet<Invoice> Invoices { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=LawOfficeDB;Trusted_Connection=True;");

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        modelBuilder.Entity<Client>()
            .HasMany<Case>()
            .WithOne(c => c.Client)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Case>()
            .HasMany(c => c.Documents)
            .WithOne()
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Client)
            .WithMany()
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Case)
            .WithMany()
             .OnDelete(DeleteBehavior.Restrict);

    }
}