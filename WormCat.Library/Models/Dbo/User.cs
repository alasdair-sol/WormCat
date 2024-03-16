using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WormCat.Library.Models.Dbo
{
    public class User
    {
        // TODO -
        // Currently The relationship is going from locaiton/container/record > User
        // It should go User ? locaiton/container/record 
        // So we can just grab the current user and fetch all their locaiton/container/record 
        // Also -
        // Add LinkedIds param here, and we can fetch all the linked accounts
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        
        public string CustomUsername { get; set; }
        public string Email { get; set; }

        public virtual List<Location>? Locations { get; set; }
        public virtual List<Container>? Containers { get; set; }
        public virtual List<Record>? Records { get; set; }
        public virtual UserGroup? UserGroup { get; set; }
    }
}
