using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILocationAccess locationAccess;
        private readonly IErrorCodeService _errorCodeService;

        [FromQuery]
        public string? query { get; set; }

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context, ILocationAccess locationAccess, IErrorCodeService errorCodeService)
        {
            _context = context;
            this.locationAccess = locationAccess;
            _errorCodeService = errorCodeService;
        }

        public IList<Location> Location { get;set; } = default!;

        public string RedirectDeleteUrl { get; protected set; }

        public async Task OnGetAsync(int? ec)
        {
            RedirectDeleteUrl = "/Locations";

            if (ec != null)
                ModelState.AddModelError(string.Empty, _errorCodeService.GetErrorMessage(ec));

            Location = await locationAccess.GetAllForUserAsync(User.Identity.GetUserId(), true);
        }
    }
}
