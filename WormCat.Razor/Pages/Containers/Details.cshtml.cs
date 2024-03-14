using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models.Dbo;

namespace WormCat.Razor.Pages.Containers
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public DetailsModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        public Container Container { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var container = await _context.Container.FirstOrDefaultAsync(m => m.Id == id);
            if (container == null)
            {
                return NotFound();
            }
            else
            {
                Container = container;
            }
            return Page();
        }
    }
}
