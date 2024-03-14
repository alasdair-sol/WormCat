using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;
using WormCat.Razor.Models;

namespace WormCat.Razor.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordAccess recordAccess;
        private readonly SeedDatabase _seedDatabase;

        [BindProperty(SupportsGet = true)]
        public string? action { get; set; } = string.Empty;

        [FromQuery]
        public string? Sort { get; set; } = string.Empty;

        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>() {
            new SelectListItem("Title", "title_asc"),
            new SelectListItem("Title (des)", "title_desc"),
            new SelectListItem("Author", "author_asc"),
            new SelectListItem("Author (des)", "author_desc")
        };

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, IRecordAccess recordAccess, SeedDatabase seedDatabase)
        {
            _logger = logger;
            _context = context;
            this.recordAccess = recordAccess;
            this._seedDatabase = seedDatabase;
        }

        public IList<Record> Records { get; set; } = new List<Record>();

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync");

            //if (User.Identity.IsAuthenticated)
            //{
            //    await _context.UserGroups.AddAsync(new UserGroup()
            //    {
            //        UserId = User.Identity.GetUserId(),
            //        OtherUserIds = new List<string> { User.Identity.GetUserId(), User.Identity.GetUserId() }
            //    });

            //    await _context.SaveChangesAsync();
            //}

            try
            {
                // Use IQueryable to reduce database reads
                //IQueryable<Record> recordsQuery = _context.Record.Include(x => x.Books!).ThenInclude(x => x.Container).ThenInclude(x => x.Location);

                Records = await recordAccess.GetAllForUserAsync(User.Identity.GetUserId(), true);

                /*var query = from record in _context.Set<Record>()
                            join book in _context.Set<Book>() on record.Id equals book.RecordId
                            join con in _context.Set<Container>() on book.ContainerId equals con.Id
                            join loc in _context.Set<Location>() on con.LocationId equals loc.Id
                            group record by loc.UserIds
                            into g
                            where g.Key
                            select record;*/

                //SortRecordQuery(ref recordsQuery);

                /*if (string.IsNullOrWhiteSpace(query) == false)
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
                }*/

                // Convert query into database read and send request
                //Records = await recordsQuery.ToListAsync();
            }
            catch { }

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

        public IActionResult OnGetSeedDatabase()
        {
            _logger.LogInformation("Seed");
            _seedDatabase.TrySeed(true);

            return LocalRedirect("/Index/?action=1");
        }

        public async Task Clear()
        {
            _logger.LogInformation("Clear");
            _seedDatabase.Clear();
            await OnGetAsync();
        }
    }
}
