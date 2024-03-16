using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Locations
{
    [AllowAnonymous]
    public class DeleteModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILocationAccess _locationAccess;
        private readonly IErrorCodeService _errorCodeService;

        public DeleteModel(WormCat.Data.Data.WormCatRazorContext context, ILocationAccess locationAccess, IErrorCodeService errorCodeService)
        {
            _context = context;
            _locationAccess = locationAccess;
            _errorCodeService = errorCodeService;
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == id);

            if (location == null)
            {
                return NotFound();
            }
            else
            {
                Location = location;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            returnUrl ??= "./Index";

            TaskResponseErrorCode<bool> response = await _locationAccess.DeleteLocationAsync(User.GetUserId<string>(), id);

            if (response.Result == false)
            {
                return Redirect($"{returnUrl}?ec={response.ErrorCode}");
            }

            return Redirect(returnUrl);
        }
    }
}
