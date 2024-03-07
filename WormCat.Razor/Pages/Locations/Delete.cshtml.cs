using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class DeleteModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public DeleteModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == id);

            if (location == null)
            {
                return NotFound();
            }
            else
            {
                Location = location;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.FindAsync(id);
            if (location != null)
            {
                Location = location;
                _context.Location.Remove(Location);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
