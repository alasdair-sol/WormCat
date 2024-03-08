using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Containers
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        public IList<Container> Container { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Container = await _context.Container.Include(x => x.Location).ToListAsync();
        }
    }
}
