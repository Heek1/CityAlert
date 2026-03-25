namespace CityAlert.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty; // Keycloak UserId

        public string UserName { get; set; } = string.Empty;

        public UserRole Role { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public string? FullName { get; set; }
        public string? Email { get; set; }

    }
}
