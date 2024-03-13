using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [FromQuery]
        public string? Sort { get; set; } = string.Empty;

        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>() {
            new SelectListItem("Title", "title_asc"),
            new SelectListItem("Title (des)", "title_desc"),
            new SelectListItem("Author", "author_asc"),
            new SelectListItem("Author (des)", "author_desc")
        };

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, IRecordUtility recordUtility)
        {
            _logger = logger;
            _context = context;
            _recordUtility = recordUtility;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync");

            // Use IQueryable to reduce database reads
            IQueryable<Record> recordsQuery = _context.Record.Include(x => x.Books!).ThenInclude(x => x.Container).ThenInclude(x => x.Location);
            SortRecordQuery(ref recordsQuery);

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                var isbnRecord = await _context.Record.Where(x => x.ISBN.ToString() == query).FirstOrDefaultAsync();

                if (isbnRecord != null)
                {
                    // If the search query matches an existing barcode, redirect to view that copy specifically.
                    return LocalRedirect($"/Records/Details?id={isbnRecord.Id}");
                }

                var book = await _context.Book.Where(x => x.Barcode == query).FirstOrDefaultAsync();

                if (book != null)
                {
                    // If the search query matches an existing barcode, redirect to view that copy specifically.
                    return LocalRedirect($"/Books/Details?id={book.Id}");
                }

                recordsQuery = recordsQuery.Where(s => s.Title.ToLower().Contains(query.ToLower()));
            }

            // Convert query into database read and send request
            Records = await recordsQuery.ToListAsync();

            return Page();
        }

        private void SortRecordQuery(ref IQueryable<Record> recordsQuery)
        {
            switch (Sort)
            {
                case "title_asc":
                    recordsQuery = recordsQuery.OrderBy(x => x.Title).ThenBy(x => x.Author);
                    break;

                case "title_desc":
                    recordsQuery = recordsQuery.OrderByDescending(x => x.Title).ThenBy(x => x.Author);
                    break;

                case "author_asc":
                    recordsQuery = recordsQuery.OrderBy(x => x.Author).ThenBy(x => x.Title);
                    break;

                case "author_desc":
                    recordsQuery = recordsQuery.OrderByDescending(x => x.Author).ThenBy(x => x.Title);
                    break;

                default:
                    recordsQuery = recordsQuery.OrderBy(x => x.Title);
                    break;
            }
        }
    }
}
