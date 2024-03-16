using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;
using WormCat.Razor.Models;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordAccess _recordAccess;
        private readonly SeedDatabase _seedDatabase;

        [BindProperty(SupportsGet = true)]
        public string? action { get; set; } = string.Empty;

        [FromQuery]
        public string? Sort { get; set; } = string.Empty;
        [FromQuery]
        public string? GroupFilter { get; set; } = string.Empty;

        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> GroupFilterOptions { get; set; } = new List<SelectListItem>();

        public IndexModel(ILogger<IndexModel> logger, WormCat.Data.Data.WormCatRazorContext context, IRecordAccess recordAccess, SeedDatabase seedDatabase)
        {
            _logger = logger;
            _context = context;
            this._recordAccess = recordAccess;
            this._seedDatabase = seedDatabase;
        }

        public IList<Record> Records { get; set; } = new List<Record>();

        public async Task<IActionResult> OnGetAsync()
        {
            SortOptions = await _recordAccess.GetSortOptions();
            GroupFilterOptions = await _recordAccess.GetGroupFilterOptions(User.GetUserId<string>());

            Records = await _recordAccess.Search(User.GetUserId<string>(), null, Sort, GroupFilter);

            return Page();
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
