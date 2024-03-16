namespace WormCat.Razor.Models
{
    /// <summary>
    /// Used to display the groups a user has access to
    /// </summary>
    public class UserGroupAccessDisplay
    {
        public int Id { get; set; }

        public string? RootCustomUsername { get; set; }
    }
}