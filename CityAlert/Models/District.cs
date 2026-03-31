using System.Text.Json.Serialization;

namespace CityAlert.Models
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>();

        [JsonIgnore]
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}