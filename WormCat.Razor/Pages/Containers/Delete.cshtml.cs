using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Containers
{
    public class DeleteModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public DeleteModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var container = await _context.Container.FindAsync(id);
            if (container != null)
            {
                Container = container;
                _context.Container.Remove(Container);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
