namespace CityAlert.Models
{
    public class DistrictListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EventsCount { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}