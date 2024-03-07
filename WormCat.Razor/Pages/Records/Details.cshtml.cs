using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Records
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(WormCat.Data.Data.WormCatRazorContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public Record Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Record.Include(x=> x.Books).ThenInclude(x=>x.Container).ThenInclude(x=>x.Location).FirstOrDefaultAsync(m => m.Id == id);
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

        public async Task<IActionResult> OnGetCreateNewCopy(int? id)
        {
            _logger.LogInformation("OnPostQuickCopy");

            if (id == null) return NotFound();

            Book book = new Book()
            {
                RecordId = (int)id,
                Container = await _context.Container.FirstOrDefaultAsync()
            };

            EntityEntry<Book> entityEntry = _context.Book.Add(book);
            
            await _context.SaveChangesAsync();

            return LocalRedirect($"/Books/Edit?id={entityEntry.Entity.Id}");
        }
    }
}
