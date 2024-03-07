using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Records
{
    public class DeleteModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public DeleteModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Record Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Record.FirstOrDefaultAsync(m => m.Id == id);

            if (record == null)
            {
                return NotFound();
            }
            else
            {
                Record = record;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Record.FindAsync(id);
            if (record != null)
            {
                Record = record;
                _context.Record.Remove(Record);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
