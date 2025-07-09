using Microsoft.EntityFrameworkCore;

namespace DiscountCodeServer.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) { }

        public DbSet<DiscountCode> DiscountCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscountCode>()
                .HasIndex(d => d.Code)
                .IsUnique();
        }
    }
}