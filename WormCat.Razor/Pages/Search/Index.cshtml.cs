using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;
using WormCat.Library.Utility;
using WormCat.Razor.Models;

namespace WormCat.Razor.Pages.Search
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordUtility _recordUtility;

        public IList<Record> Records { get; set; } = new List<Record>();

        [BindProperty(SupportsGet = true)]
        public string? query { get; set; } = string.Empty;

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, IRecordUtility recordUtility)
        {
            _logger = logger;
            _context = context;
            _recordUtility = recordUtility;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Search.OnGetAsync");

            Records = await _context.Record.Include(x => x.Books).ThenInclude(x => x.Container).ThenInclude(x => x.Location).ToListAsync();

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                if (_recordUtility.IsISBN(query, out string isbn))
                {
                    var isbnRecord = await _context.Record.Where(x => x.ISBN.ToString() == query).FirstOrDefaultAsync();

                    if (isbnRecord != null)
                    {
                        // If the search query matches an existing barcode, redirect to view that copy specifically.
                        return LocalRedirect($"/Records/Details?id={isbnRecord.Id}");
                    }
                    else
                    {
                        return LocalRedirect($"/Records/Create?query={isbn}&handler=CreateFromContentProvider");
                    }
                }

                var book = await _context.Book.Where(x => x.Barcode == query).FirstOrDefaultAsync();

                if (book != null)
                {
                    // If the search query matches an existing barcode, redirect to view that copy specifically.
                    return LocalRedirect($"/Books/Details?id={book.Id}");
                }

                Records = Records.Where(s => s.Title.ToLower().Contains(query.ToLower())).ToList();
            }

            return Page();
        }
    }
}
