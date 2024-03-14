using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models.Dbo
{
    public class Location
    {
        public int Id { get; set; }

        public virtual string? UserId { get; set; }

        public virtual User? User { get; set; }

        [Required]
        [MinLength(4)]
        public string? Name { get; set; }

        [Display(Name = "Street No.")]
        public string? StreetNumber { get; set; }

        [Display(Name = "Street Name")]
        [MinLength(4)]
        public string? StreetName { get; set; }

        [MinLength(4)]
        public string? Town { get; set; }

        [MinLength(4)]
        public string? City { get; set; }

        [Display(Name = "Post Code")]
        public string? PostCode { get; set; }
    }
}
