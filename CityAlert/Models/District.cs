namespace CityAlert.Models
{
    public class District
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; } = new List<Event>();

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    }
}
