using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;
using WormCat.Razor.Models;

namespace WormCat.Razor.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly SeedDatabase _seedDatabase;

        [BindProperty(SupportsGet = true)]
        public string? query { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? action { get; set; } = string.Empty;

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, SeedDatabase seedDatabase)
        {
            _logger = logger;
            _context = context;
            this._seedDatabase = seedDatabase;
        }

        public IList<Record> Records { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync");

            Records = await _context.Record.Include(x => x.Books).ThenInclude(x => x.Container).ThenInclude(x => x.Location).ToListAsync();

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                var book = await _context.Book.Where(x => x.Barcode == query).FirstOrDefaultAsync();

                if(book != null)
                {
                    // If the search query matches an existing barcode, redirect to view that copy specifically.
                    return LocalRedirect($"/Books/Details?id={book.Id}");
                }

                Records = Records.Where(s => s.Title.ToLower().Contains(query.ToLower())).ToList();
            }

            return Page();
        }

        public IActionResult OnGetSeedDatabase()
        {
            _logger.LogInformation("Seed");
            _seedDatabase.TrySeed(true);

            return LocalRedirect("/?action=1");
        }

        public async Task Clear()
        {
            _logger.LogInformation("Clear");
            _seedDatabase.Clear();
            await OnGetAsync();
        }
    }
}
