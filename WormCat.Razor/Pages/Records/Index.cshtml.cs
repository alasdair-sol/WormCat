using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Razor.Pages.Records
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;

        public IndexModel(WormCat.Data.Data.WormCatRazorContext context)
        {
            _context = context;
        }

        public IList<Record> Records { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Records = await _context.Record.ToListAsync();
        }
    }
}
