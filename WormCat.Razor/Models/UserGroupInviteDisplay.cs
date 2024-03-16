namespace WormCat.Razor.Models
{
    public class UserGroupInviteDisplay
    {
        public int Id { get; set; }
        public string UserIdFrom { get; set; } = string.Empty;
        public string UserIdTo { get; set; } = string.Empty;

        public string? UsernameFrom { get; set; }
        public string? UsernameTo { get; set; }
    }
}
