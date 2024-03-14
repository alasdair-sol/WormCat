using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILocationAccess locationAccess;

        [FromQuery]
        public string? query { get; set; }

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context, ILocationAccess locationAccess)
        {
            _context = context;
            this.locationAccess = locationAccess;
        }

        public IList<Location> Location { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Location = await locationAccess.GetAllForUserAsync(User.Identity.GetUserId());
        }
    }
}
