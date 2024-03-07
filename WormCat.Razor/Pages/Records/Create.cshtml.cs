using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public CreateModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Record Record { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Record.Add(Record);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
