using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;

namespace WormCat.Razor.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IGenericService _genericUtility;

        public IEnumerable<SelectListItem>? Containers;
        public IEnumerable<SelectListItem>? Records;

        public CreateModel(WormCat.Data.Data.WormCatRazorContext context, IGenericService genericUtility)
        {
            _context = context;
            _genericUtility = genericUtility;
        }

        public IActionResult OnGetAsync()
        {
            PopulateSelectLists();

            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Attach to new Record")]
        public bool CreateRecordOnFly { get; set; } = false;

        //public bool CreateRecord

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            PopulateSelectLists();

            if (CreateRecordOnFly)
            {
                if (string.IsNullOrWhiteSpace(Book.Record?.Title))
                    return Page();

                EntityEntry<Record> recordEntity = _context.Record.Add(new Record() { Title = Book.Record.Title });

                Book.RecordId = recordEntity.Entity.Id;
                Book.Record = recordEntity.Entity;
            }
            else
            {
                Book.Record = _context.Record.Where(x => x.Id == Book.RecordId).FirstOrDefault();
            }

            Book.Container = _context.Container.Where(x => x.Id == Book.ContainerId).FirstOrDefault();

            List<ModelErrorCollection?> errors = ModelState.Select(x => x.Value?.Errors).Where(x => x.Count > 0).ToList();

            if (CreateRecordOnFly == false)
                ModelState.Remove("Book.Record.Title");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Book.Add(Book);
            await _context.SaveChangesAsync();

            return LocalRedirect($"/Records/Details?id={Book.RecordId}");
        }

        private void PopulateSelectLists()
        {
            Records = _genericUtility.GetSelectList(_context.Record, null, t => t.Title ?? string.Empty, t => t.Id.ToString());
            Containers = _genericUtility.GetSelectList(_context.Container, null, t => t.Name ?? string.Empty, t => t.Id.ToString());
        }
    }
}
