namespace CityAlert.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public EventCategory Category { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; } = null!;

        public SeverityLevel Severity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
