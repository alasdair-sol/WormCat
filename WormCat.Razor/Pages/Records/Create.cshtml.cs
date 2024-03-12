using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WormCat.Library.Models;
using WormCat.Library.Utility;

namespace WormCat.Razor.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordUtility _recordUtility;
        private readonly IEnrichedContentProvider _enrichedContentProvider;

        public CreateModel(WormCat.Data.Data.WormCatRazorContext context, IRecordUtility recordUtility, IEnrichedContentProvider enrichedContentProvider)
        {
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

        public IActionResult OnGetCreateFromContentProvider(string? query)
        {
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                Record.ISBN = query;

                EnrichedContentModel? enrichedContentModel = _enrichedContentProvider.GetEnrichedContent(query);

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            EntityEntry<Book> book = _context.Book.Add(new Book { RecordId = Record.Id, ContainerId = _context.Container?.FirstOrDefault()?.Id ?? -1 });

            Record.Books = new List<Book>() { book.Entity };

            EntityEntry<Record> entityEntry = _context.Record.Add(Record);

            await _context.SaveChangesAsync();

            return LocalRedirect($"/Records/Details?id={entityEntry.Entity.Id}");
        }
    }
}
