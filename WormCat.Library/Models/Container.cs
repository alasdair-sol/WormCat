using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models
{
    public class Container
    {
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        public string? Name { get; set; }

        [Required]
        public virtual int LocationId { get; set; }

        public virtual Location? Location { get; set; }
    }
}
