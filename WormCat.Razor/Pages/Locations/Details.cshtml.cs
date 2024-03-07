using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public DetailsModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

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
    }
}
