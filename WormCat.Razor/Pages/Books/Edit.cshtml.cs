using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;
using WormCat.Library.Utility;

namespace WormCat.Razor.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IGenericUtility _genericUtility;
        public IEnumerable<SelectListItem>? Containers;
        public IEnumerable<SelectListItem>? Records;

        [BindProperty]
        public Book Book { get; set; } = default!;

        public EditModel(ILogger<EditModel> logger, WormCat.Data.Data.WormCatRazorContext context, IGenericUtility genericUtility)
        {
            _context = context;
            _logger = logger;
            _genericUtility = genericUtility;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.Include(x => x.Record).Include(x=>x.Container).FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            Book = book;

            Records = _genericUtility.GetSelectList(_context.Record, book.RecordId, t => t.Title ?? string.Empty, t => t.Id.ToString());
            Containers = _genericUtility.GetSelectList(_context.Container, book.ContainerId, t => t.Name ?? string.Empty, t => t.Id.ToString());

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Book.Record = _context.Record.Where(x => x.Id == Book.RecordId).FirstOrDefault();
            Book.Container = _context.Container.Where(x => x.Id == Book.ContainerId).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return Page();
            }            

            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return LocalRedirect($"/Records/Details?id={Book.RecordId}");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
