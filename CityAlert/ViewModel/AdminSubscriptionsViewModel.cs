using CityAlert.Models;

namespace CityAlert.Models
{
    public class AdminSubscriptionsViewModel
    {
        public int TotalSubscriptions { get; set; }
        public int UniqueUsersCount { get; set; }
        public List<DistrictSubscriptionCount> SubscriptionsByDistrict { get; set; } = new();
        public List<AdminSubscriptionItemViewModel> AllSubscriptions { get; set; } = new();
    }

    public class AdminSubscriptionItemViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
    }
}
