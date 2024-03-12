using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models
{
    public class Record
    {
        public int Id { get; set; }

        public virtual List<Book>? Books { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Synopsis { get; set; }

        public int? PageCount { get; set; }

        public string? PublicationDate { get; set; }

        public string? Author { get; set; }

        [StringLength(maximumLength: 13, MinimumLength = 13)]
        public string? ISBN { get; set; }

        public string? Image { get; set; }
    }
}
