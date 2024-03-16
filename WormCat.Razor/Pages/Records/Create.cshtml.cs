using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WormCat.Data.DataAccess;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Library.Services.Interfaces;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> logger;
        private readonly IRecordAccess recordAccess;
        private readonly WormCat.Data.Data.WormCatRazorContext _context;
        private readonly IRecordUtility _recordUtility;
        private readonly IEnrichedContentService _enrichedContentProvider;
        private readonly IErrorCodeService _errorCodeService;
        private readonly IContainerAccess _containerAccess;
        private readonly IGenericService _genericUtility;

        public IEnumerable<SelectListItem>? Containers;

        [BindProperty(SupportsGet = true)]
        public Container ChosenContainer { get; set; }

        public CreateModel(ILogger<CreateModel> logger, IRecordAccess recordAccess,
            WormCat.Data.Data.WormCatRazorContext context, IRecordUtility recordUtility, IEnrichedContentService enrichedContentProvider,
            IErrorCodeService errorCodeService, IContainerAccess containerAccess, IGenericService _genericUtility)
        {
            this.logger = logger;
            this.recordAccess = recordAccess;
            _context = context;
            _recordUtility = recordUtility;
            _enrichedContentProvider = enrichedContentProvider;
            _errorCodeService = errorCodeService;
            _containerAccess = containerAccess;
            this._genericUtility = _genericUtility;
        }

        private async Task RetrieveData()
        {
            var containers = await _containerAccess.GetAllForUserAsync(User.GetUserId<string>(), true);

            Containers = _genericUtility.GetSelectList<Container>(containers, containers.FirstOrDefault()?.Id ?? -1, (t) =>
            {
                if (t.UserId == User.GetUserId<string>())
                    return t.Name ?? string.Empty;
                else
                    return $"{t.Name} [{t.User.CustomUsername}]";
            }, t => t.Id.ToString());
        }

        public async Task<IActionResult> OnGet()
        {
            await RetrieveData();
            return Page();
        }

        public async Task<IActionResult> OnGetCreateFromSearchQuery(string? query)
        {
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                if (_recordUtility.IsISBN(query, out string isbn))
                    Record.ISBN = isbn;
                else Record.Title = query;
            }

            await RetrieveData();
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

            await RetrieveData();
            return Page();
        }

        [BindProperty]
        public Record Record { get; set; } = new Record();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await RetrieveData();

            ModelState.Remove("Record.UserIds");
            ModelState.Remove("ChosenContainer.Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            TaskResponseErrorCode<Record?> response = await recordAccess.CreateNewAsyncWithDefaultBook(ChosenContainer.Id, Record);

            if (response.Result == null)
            {
                ModelState.AddModelError(string.Empty, _errorCodeService.GetErrorMessage(response.ErrorCode));
                return Page();
            }

            return LocalRedirect($"/Records/Details?id={response.Result.Id}");
        }
    }
}
