using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Containers
{
    public class EditModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public EditModel(WormCat.Data.Data.WormCatRazorContext context)
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

            var container =  await _context.Container.FirstOrDefaultAsync(m => m.Id == id);
            if (container == null)
            {
                return NotFound();
            }
            Container = container;
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

            _context.Attach(Container).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContainerExists(Container.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ContainerExists(int id)
        {
            return _context.Container.Any(e => e.Id == id);
        }
    }
}
