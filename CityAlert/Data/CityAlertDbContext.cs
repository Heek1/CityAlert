using CityAlert.Models;
using Microsoft.EntityFrameworkCore;

namespace CityAlert.Data
{
    public class CityAlertDbContext : DbContext
    {
        public CityAlertDbContext(DbContextOptions<CityAlertDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.District)
                .WithMany(d => d.Events)
                .HasForeignKey(e => e.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.District)
                .WithMany(d => d.Subscriptions)
                .HasForeignKey(s => s.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => new { s.UserId, s.DistrictId })
                .IsUnique();

            modelBuilder.Entity<District>().HasData(
                new District { Id = 1, Name = "Центр" },
                new District { Id = 2, Name = "Пасічна" },
                new District { Id = 3, Name = "Вовчинець" },
                new District { Id = 4, Name = "Позитрон" },
                new District { Id = 5, Name = "Крихівці" }
            );
        }
    }
}