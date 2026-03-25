using Microsoft.EntityFrameworkCore;

namespace CityAlert.Data
{
    public class CityAlertDbContext : DbContext
    {
        public CityAlertDbContext(DbContextOptions<CityAlertDbContext> options) : base(options)
        {
        }
    }
}
