namespace CityAlert.Models
{
    public class AdminPanelViewModel
    {
        public int TotalActiveEvents { get; set; }
        public int TotalSubscriptions { get; set; }
        public int UniqueSubscribers { get; set; }
        public List<DistrictSubscriptionCount> SubscriptionsByDistrict { get; set; } = new();
        public List<Event> AllEvents { get; set; } = new();
    }

    public class DistrictSubscriptionCount
    {
        public string DistrictName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
