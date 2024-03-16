using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Locations
{
    public class CreateModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly ILocationAccess _locationAccess;
        private readonly IErrorCodeService _errorCodeService;

        public CreateModel(WormCat.Data.Data.WormCatRazorContext context, ILocationAccess locationAccess, IErrorCodeService errorCodeService)
        {
            _context = context;
            _locationAccess = locationAccess;
            _errorCodeService = errorCodeService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TaskResponseErrorCode<Location?> response = await _locationAccess.CreateNewAsync(User.GetUserId<string>(), Location);

            if (response.Result == null)
            {
                ModelState.AddModelError(string.Empty, _errorCodeService.GetErrorMessage(response.ErrorCode));
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
