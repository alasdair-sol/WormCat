using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services;

namespace WormCat.Razor.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> logger;
        private readonly IRecordAccess recordAccess;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordUtility _recordUtility;
        private readonly IEnrichedContentService _enrichedContentProvider;

        public CreateModel(ILogger<CreateModel> logger, IRecordAccess recordAccess, WormCat.Data.Data.WormCatRazorContext context, IRecordUtility recordUtility, IEnrichedContentService enrichedContentProvider)
        {
            this.logger = logger;
            this.recordAccess = recordAccess;
            _context = context;
            _recordUtility = recordUtility;
            _enrichedContentProvider = enrichedContentProvider;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetCreateFromSearchQuery(string? query)
        {
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                if (_recordUtility.IsISBN(query, out string isbn))
                    Record.ISBN = isbn;
                else Record.Title = query;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetCreateFromContentProvider(string? query)
        {
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                Record.ISBN = query;

                EnrichedContentModel? enrichedContentModel = await _enrichedContentProvider.GetEnrichedContentAsync(query);

                if (enrichedContentModel != null)
                {
                    Record.Title = enrichedContentModel.Title;
                    Record.Author = enrichedContentModel.Author;
                    Record.PublicationDate = enrichedContentModel.PublicationDate;
                    Record.PageCount = enrichedContentModel.PageCount;
                    Record.Synopsis = enrichedContentModel.Synopsis;
                    Record.Image = enrichedContentModel.Image;
                }
            }

            return Page();
        }

        [BindProperty]
        public Record Record { get; set; } = new Record();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Record.UserIds");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Record? newRecord = await recordAccess.CreateNewAsync(User.Identity.GetUserId(), Record);

            if (newRecord == null)
                return Page();

            await recordAccess.SaveContextAsync();

            return LocalRedirect($"/Records/Details?id={newRecord.Id}");
        }
    }
}
