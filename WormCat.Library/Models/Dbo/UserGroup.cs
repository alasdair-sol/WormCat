using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WormCat.Library.Models.Dbo
{
    public class UserGroup
    {
        public int Id { get; set; }

        [Required]
        public virtual string UserId { get; set; }

        // The Root user for this group
        public virtual User? User { get; set; }

        // All other users in this group
        public List<string>? OtherUserIds { get; set; } = default!;
    }
}
