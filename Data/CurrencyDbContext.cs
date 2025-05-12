// Data/CurrencyDbContext.cs
using api.Entities;
using Microsoft.EntityFrameworkCore;


public class CurrencyDbContext : DbContext
{
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<CurrencyRate> CurrencyRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<CurrencyRate>()
            .HasKey(cr => cr.Id);

        modelBuilder.Entity<CurrencyRate>()
            .HasOne(cr => cr.Currency)
            .WithMany(c => c.CurrencyRates)
            .HasForeignKey(cr => cr.CurrencyId);
    }
}
