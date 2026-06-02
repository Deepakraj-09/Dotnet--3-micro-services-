using Microsoft.EntityFrameworkCore;
using ShippingService.Models;

namespace ShippingService.Data
{
    public class ShippingDbContext : DbContext
    {
        public ShippingDbContext(DbContextOptions<ShippingDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Code> Codes => Set<Code>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().ToTable("cities");
            modelBuilder.Entity<Code>().ToTable("codes");
        }
    }
}
