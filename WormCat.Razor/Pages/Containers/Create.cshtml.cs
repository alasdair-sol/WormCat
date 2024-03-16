using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Containers
{
    public class CreateModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IContainerAccess _containerAccess;
        private readonly ILocationAccess _locationAccess;
        private readonly IGenericService _genericService;

        [BindProperty]
        public Container Container { get; set; }

        public IEnumerable<SelectListItem>? LocationSelects;

        public CreateModel(WormCat.Data.Data.WormCatRazorContext context, IContainerAccess containerAccess, ILocationAccess locationAccess, IGenericService genericService)
        {
            _context = context;
            _containerAccess = containerAccess;
            _locationAccess = locationAccess;
            _genericService = genericService;
        }

        public async Task<IActionResult> OnGet()
        {
            await RetrieveData();

            return Page();
        }

        private async Task RetrieveData()
        {
            string userId = User.GetUserId<string>();
            var locations = await _locationAccess.GetAllForUserAsync(userId, true);

            LocationSelects = _genericService.GetSelectList<Location>(locations, locations.First().Id, t =>
            {
                if (t.UserId == userId)
                {
                    return t.Name;
                }
                else
                {
                    return $"{t.Name} [{t.User.CustomUsername}]";
                }

            }, t => t.Id.ToString()).ToList();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await RetrieveData();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _containerAccess.CreateNewAsync(Container);

            _context.Container.Add(Container);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
