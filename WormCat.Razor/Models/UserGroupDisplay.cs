using WormCat.Library.Models.Dbo;

namespace WormCat.Razor.Models
{
    /// <summary>
    /// Used to display a specific user's Group, and the users that belong to it
    /// </summary>
    public class UserGroupDisplay
    {
        public int GroupId { get; set; }

        public string RootUserId { get; set; } = string.Empty;
        public string RootCustomUsername { get; set; } = string.Empty;

        public string AccessUserId { get; set; } = string.Empty;
        public string AccessCustomUsername { get; set; } = string.Empty;

        //public List<(string userId, string username)> UsersWhoHaveAccess { get; set; } = new List<(string userId, string username)>();
    }
}