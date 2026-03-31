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

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Прорив магістрального водопроводу",
                    Description = "У зв'язку з аварією на вул. Галицькій, водопостачання буде відсутнє до 20:00. Працює аварійна бригада.",
                    Category = EventCategory.Infrastructure,
                    DistrictId = 1,
                    Severity = SeverityLevel.Critical,
                    CreatedAt = new DateTime(2025, 1, 10, 8, 0, 0),
                    StartDate = new DateTime(2025, 1, 10, 8, 30, 0),
                    EndDate = new DateTime(2025, 1, 11, 20, 0, 0),
                    CreatedBy = "system_bot",
                    IsActive = true
                },
                new Event
                {
                    Id = 2,
                    Title = "Зміна маршруту автобуса №22",
                    Description = "Через проведення дорожніх робіт автобус тимчасово курсуватиме через вул. Тролейбусну.",
                    Category = EventCategory.Transport,
                    DistrictId = 2,
                    Severity = SeverityLevel.Warning,
                    CreatedAt = new DateTime(2025, 1, 11, 12, 0, 0),
                    StartDate = new DateTime(2025, 1, 12, 6, 0, 0),
                    EndDate = new DateTime(2025, 1, 15, 23, 0, 0),
                    CreatedBy = "admin_moderator",
                    IsActive = true
                },
                new Event
                {
                    Id = 3,
                    Title = "Планове відключення світла",
                    Description = "Ремонтні роботи на підстанції. Світла не буде у приватному секторі.",
                    Category = EventCategory.Infrastructure,
                    DistrictId = 3,
                    Severity = SeverityLevel.Info,
                    CreatedAt = new DateTime(2025, 1, 12, 9, 0, 0),
                    StartDate = new DateTime(2025, 1, 13, 9, 0, 0),
                    EndDate = new DateTime(2025, 1, 13, 17, 0, 0),
                    CreatedBy = "energy_service",
                    IsActive = true
                },
                new Event
                {
                    Id = 4,
                    Title = "Благодійний ярмарок",
                    Description = "Запрошуємо всіх мешканців на ярмарок у підтримку ЗСУ.",
                    Category = EventCategory.Culture,
                    DistrictId = 5,
                    Severity = SeverityLevel.Info,
                    CreatedAt = new DateTime(2025, 1, 13, 10, 0, 0),
                    StartDate = new DateTime(2025, 1, 20, 11, 0, 0),
                    EndDate = new DateTime(2025, 1, 20, 18, 0, 0),
                    CreatedBy = "local_community",
                    IsActive = true
                },
                new Event
                {
                    Id = 5,
                    Title = "ДТП на перехресті",
                    Description = "Аварія біля ТЦ. Рух ускладнено.",
                    Category = EventCategory.Transport,
                    DistrictId = 4,
                    Severity = SeverityLevel.Warning,
                    CreatedAt = new DateTime(2025, 1, 14, 15, 0, 0),
                    StartDate = new DateTime(2025, 1, 14, 15, 10, 0),
                    EndDate = new DateTime(2025, 1, 14, 18, 0, 0),
                    CreatedBy = "traffic_police",
                    IsActive = true
                }
            );
        }
    }
}