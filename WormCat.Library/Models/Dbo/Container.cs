using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models.Dbo
{
    public class Container
    {
        public int Id { get; set; }

        public virtual string? UserId { get; set; }

        public virtual User? User { get; set; }

        [Required]
        [MinLength(4)]
        public string? Name { get; set; }

        [Required]
        public virtual int LocationId { get; set; }

        public virtual Location? Location { get; set; }

        public virtual List<Book>? Books { get; set; }
    }
}
