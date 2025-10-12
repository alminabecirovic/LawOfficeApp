using LawOfficeApp.Data;
using LawOfficeApp.Models;

namespace LawOfficeApp.Services;

public static class DataSeeder
{
    public static void SeedIfEmpty(LawOfficeDbContext ctx)
    {
        if (ctx.Advokati.Any()) return;

        var c1 = new Client("Petar", "Perić", "petar@mail.com");
        var c2 = new Client("Jelena", "Milić", "jelena@mail.com");

        var case1 = new Case("Razvod", "Open", DateTime.Now.AddDays(10), c1);
        var case2 = new Case("Ugovor o radu", "Open", DateTime.Now.AddDays(25), c2);

        var a1 = new Advokat("Ana", "Petrović", "Porodično pravo");
        var a2 = new Advokat("Marko", "Janković", "Poslovno pravo");

        a1.ActiveCases.Add(case1);
        a2.ActiveCases.Add(case2);

        ctx.Klijenti.AddRange(c1, c2);
        ctx.Slucajevi.AddRange(case1, case2);
        ctx.Advokati.AddRange(a1, a2);

        ctx.SaveChanges();
    }
}
