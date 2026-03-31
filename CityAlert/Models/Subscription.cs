namespace CityAlert.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty; 

        public int DistrictId { get; set; }
        public District District { get; set; } = null!;

    }
} 
