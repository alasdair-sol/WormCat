using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Razor.Pages.Containers
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IContainerAccess containerAccess;

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context, IContainerAccess containerAccess)
        {
            _context = context;
            this.containerAccess = containerAccess;
        }

        public IList<Container> Container { get; set; } = default!;

        public async Task OnGetAsync(string userId)
        {
            Container = await containerAccess.GetAllForUserAsync(User.Identity.GetUserId(), true);
        }
    }
}
