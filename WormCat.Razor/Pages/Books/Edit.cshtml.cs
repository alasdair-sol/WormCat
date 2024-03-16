using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IGenericService _genericUtility;
        private readonly IContainerAccess _containerAccess;
        private readonly IBookAccess _bookAccess;
        private readonly IErrorCodeService _errorCodeService;

        public IEnumerable<SelectListItem>? Containers;
        public IEnumerable<SelectListItem>? Records;

        [BindProperty]
        public Book Book { get; set; } = default!;

        public EditModel(ILogger<EditModel> logger, WormCat.Data.Data.WormCatRazorContext context, IGenericService genericUtility,
            IContainerAccess containerAccess, IBookAccess bookAccess, IErrorCodeService errorCodeService)
        {
            _context = context;
            _logger = logger;
            _genericUtility = genericUtility;
            _containerAccess = containerAccess;
            _bookAccess = bookAccess;
            _errorCodeService = errorCodeService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.Include(x => x.Record).Include(x => x.Container).FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            Book = book;

            await RetrieveData(book);
            return Page();
        }

        private async Task RetrieveData(Book? book)
        {
            var containers = await _containerAccess.GetAllForUserAsync(User.GetUserId<string>(), true);

            Containers = _genericUtility.GetSelectList<Container>(containers, book.ContainerId, (t) =>
            {
                if (t.UserId == User.GetUserId<string>())
                    return t.Name ?? string.Empty;
                else
                    return $"{t.Name} [{t.User.CustomUsername}]";
            }, t => t.Id.ToString());
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TaskResponseErrorCode<bool> response = await _bookAccess.UpdateBookAsync(Book);

            if(response.Result == false)
            {
                ModelState.AddModelError(string.Empty, _errorCodeService.GetErrorMessage(response.ErrorCode));
                return Page();
            }

            //_context.Attach(Book).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!BookExists(Book.Id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return LocalRedirect($"/Records/Details?id={Book.RecordId}");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
