using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Razor.Pages.Records
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILogger<DetailsModel> _logger;
        private readonly IBookAccess bookAccess;
        private readonly IContainerAccess containerAccess;

        public DetailsModel(WormCat.Data.Data.WormCatRazorContext context, ILogger<DetailsModel> logger, IBookAccess bookAccess, IContainerAccess containerAccess)
        {
            _context = context;
            this._logger = logger;
            this.bookAccess = bookAccess;
            this.containerAccess = containerAccess;
        }

        public Record Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.Record.Include(x => x.Books).ThenInclude(x => x.Container).ThenInclude(x => x.Location).FirstOrDefaultAsync(m => m.Id == id);
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
            if (id == null) return NotFound();       

            Book? book = await bookAccess.CreateNewForRecordAsync(User.Identity.GetUserId(), (int)id);

            if (book == null) return NotFound();

            await bookAccess.SaveContextAsync();

            return LocalRedirect($"/Books/Edit?id={book.Id}");
        }
    }
}
