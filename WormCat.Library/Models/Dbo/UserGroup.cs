using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WormCat.Library.Models.Dbo
{
    public class UserGroup
    {
        public int Id { get; set; }

        /// <summary>
        /// The user who has been given access
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        public virtual User? User { get; set; }

        /// <summary>
        /// The user accounts that [User/UserId] can access
        /// </summary>
        public List<string>? OtherUserIds { get; set; } = default!;
    }
}
