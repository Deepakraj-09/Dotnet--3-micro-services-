using Microsoft.EntityFrameworkCore;
using RoboShop.Shipping.Models;

namespace RoboShop.Shipping.Infrastructure;

/// <summary>
/// EF Core DbContext — replaces Spring Data JPA auto-configuration + JpaConfig.java.
/// Connection string is read from DB_HOST env var (same as original Java service).
/// </summary>
public class ShippingDbContext : DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options) { }

    public DbSet<City> Cities { get; set; }
    public DbSet<Code> Codes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map Code.CodeValue property to the "code" column (avoids EF naming conflict)
        modelBuilder.Entity<Code>()
            .Property(c => c.CodeValue)
            .HasColumnName("code");

        // Map City.CityName property to the "city" column
        modelBuilder.Entity<City>()
            .Property(c => c.CityName)
            .HasColumnName("city");
    }
}
