using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Search
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordAccess _recordAccess;

        public IList<Record> Records { get; set; } = new List<Record>();

        [BindProperty(SupportsGet = true)]
        public string? query { get; set; } = string.Empty;

        [FromQuery]
        public string? Sort { get; set; } = string.Empty;
        [FromQuery]
        public string? GroupFilter { get; set; } = string.Empty;

        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> GroupFilterOptions { get; set; } = new List<SelectListItem>();

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, IRecordAccess recordAccess)
        {
            _logger = logger;
            _context = context;
            _recordAccess = recordAccess;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SortOptions = await _recordAccess.GetSortOptions();
            GroupFilterOptions = await _recordAccess.GetGroupFilterOptions(User.GetUserId<string>());

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                Record? isbnRecord = await _recordAccess.SearchForIsbn(User.GetUserId<string>(), query);

                //var isbnRecord = await _context.Record.Where(x => x.ISBN.ToString() == query).FirstOrDefaultAsync();

                if (isbnRecord != null)
                {
                    // If the search query matches an existing barcode, redirect to view that copy specifically.
                    return LocalRedirect($"/Records/Details?id={isbnRecord.Id}");
                }

                //var book = await _context.Book.Where(x => x.Barcode == query).FirstOrDefaultAsync();

                //if (book != null)
                //{
                //    // If the search query matches an existing barcode, redirect to view that copy specifically.
                //    return LocalRedirect($"/Books/Details?id={book.Id}");
                //}
            }

            Records = await _recordAccess.Search(User.GetUserId<string>(), query, Sort, GroupFilter);

            return Page();
        }


        /*
            // Use IQueryable to reduce database reads
            //IQueryable<Record> recordsQuery = _context.Record.Include(x => x.Books!).ThenInclude(x => x.Container).ThenInclude(x => x.Location);
            //SortRecordQuery(ref recordsQuery);

            //if (string.IsNullOrWhiteSpace(query) == false)
            //{
            //    var isbnRecord = await _context.Record.Where(x => x.ISBN.ToString() == query).FirstOrDefaultAsync();

            //    if (isbnRecord != null)
            //    {
            //        // If the search query matches an existing barcode, redirect to view that copy specifically.
            //        return LocalRedirect($"/Records/Details?id={isbnRecord.Id}");
            //    }

            //    var book = await _context.Book.Where(x => x.Barcode == query).FirstOrDefaultAsync();

            //    if (book != null)
            //    {
            //        // If the search query matches an existing barcode, redirect to view that copy specifically.
            //        return LocalRedirect($"/Books/Details?id={book.Id}");
            //    }

            //    recordsQuery = recordsQuery.Where(s => s.Title.ToLower().Contains(query.ToLower()));
            //}

            //// Convert query into database read and send request
            //Records = await recordsQuery.ToListAsync();

            Records = await recordAccess.Search(User.GetUserId<string>(), Sort, null);
        */
    }
}
