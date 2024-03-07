using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models
{
    public class Record
    {
        public int Id { get; set; }

        public virtual List<Book>? Books { get; set; }

        [Required]
        [MinLength(4)]
        public string? Title { get; set; }

        [MinLength(4)]
        public string? Description { get; set; }

        [StringLength(maximumLength: 13, MinimumLength = 13)]
        public string? ISBN { get; set; }

        public string? Image { get; set; }
    }
}
