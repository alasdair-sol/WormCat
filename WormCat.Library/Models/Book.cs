using System.ComponentModel.DataAnnotations;

namespace WormCat.Library.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string? Barcode { get; set; }

        [Display(Name = "On Loan")]
        public bool OnLoan { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerId { get; set; }

        [Required]
        public virtual int RecordId { get; set; }

        public virtual Record? Record { get; set; } = default!;

        [Required]
        public virtual int ContainerId { get; set; }

        public virtual Container? Container { get; set; }
    }
}
