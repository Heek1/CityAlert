using CityAlert.Models;

namespace CityAlert.Models
{
    public class ResidentSubscriptionsViewModel
    {
        public List<DistrictSubscriptionItemViewModel> Districts { get; set; } = new();
        public int TotalSubscriptions => Districts.Count(d => d.IsSubscribed);
    }

    public class DistrictSubscriptionItemViewModel
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; }
        public int SubscriberCount { get; set; }
    }
}
