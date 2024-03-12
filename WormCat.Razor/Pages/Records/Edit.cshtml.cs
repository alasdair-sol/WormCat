using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Records
{
    public class EditModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public EditModel(WormCat.Data.Data.WormCatRazorContext context)
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

            var record =  await _context.Record.FirstOrDefaultAsync(m => m.Id == id);
            if (record == null)
            {
                return NotFound();
            }
            Record = record;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(Record.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return LocalRedirect($"/Records/Details?id={Record.Id}");
        }

        private bool RecordExists(int id)
        {
            return _context.Record.Any(e => e.Id == id);
        }
    }
}
