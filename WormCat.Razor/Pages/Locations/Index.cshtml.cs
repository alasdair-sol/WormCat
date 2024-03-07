using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        [FromQuery]
        public string? query { get; set; }

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        public IList<Location> Location { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Location = await _context.Location.ToListAsync();
        }
    }
}
