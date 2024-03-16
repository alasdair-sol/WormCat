using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class RecordAccess : IRecordAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext _context;
        private readonly IContainerAccess containerAccess;
        private readonly IUserAccess userAccess;

        public RecordAccess(ILogger<UserAccess> logger, WormCatRazorContext context, IContainerAccess containerAccess, IUserAccess userAccess)
        {
            this.logger = logger;
            this._context = context;
            this.containerAccess = containerAccess;
            this.userAccess = userAccess;
        }

        public async Task<TaskResponseErrorCode<Record?>> CreateNewAsync(string userId, Record record, bool skipCopy = false)
        {
            try
            {
                var user = await userAccess.GetUserById(userId);

                if (user == null)
                    return new TaskResponseErrorCode<Record?>(null, 105);

                var container = await containerAccess.GetFirstOrDefaultForUserAsync(userId);

                if (container == null)
                    return new TaskResponseErrorCode<Record?>(null, 106);

                //var book = await bookAccess.CreateNewAsync(record.Id, container.Id);
                if (skipCopy)
                {
                    record.Books = new List<Book>();
                }
                else
                {
                    var book = await _context.Book.AddAsync(new Book { RecordId = record.Id, ContainerId = container.Id });

                    if (book.Entity == null)
                        throw new Exception("Failed to create book when creating record");

                    record.Books = new List<Book>() { book.Entity };
                }

                record.User = user;

                EntityEntry<Record> entry = _context.Record.Add(record);

                await SaveContextAsync();

                return new TaskResponseErrorCode<Record?>(entry.Entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<Record?>(null, 101);
            }
        }

        public async Task<TaskResponseErrorCode<Record?>> CreateNewAsyncWithoutDefaultBook(string? userId, Record record)
        {
            try
            {
                var user = await userAccess.GetUserById(userId ?? string.Empty);

                if (user == null)
                    return new TaskResponseErrorCode<Record?>(null, 105);

                var container = await containerAccess.GetFirstOrDefaultForUserAsync(userId!);

                if (container == null)
                    return new TaskResponseErrorCode<Record?>(null, 106);

                record.User = user;

                EntityEntry<Record> entry = _context.Record.Add(record);

                await SaveContextAsync();

                return new TaskResponseErrorCode<Record?>(entry.Entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<Record?>(null, 101);
            }
        }

        public async Task<TaskResponseErrorCode<Record?>> CreateNewAsyncWithDefaultBook(int containerIdForDefaultBook, Record record)
        {
            try
            {
                var container = await containerAccess.GetAsync(containerIdForDefaultBook);

                if (container == null)
                    return new TaskResponseErrorCode<Record?>(null, 106);

                var user = await userAccess.GetUserById(container.UserId ?? string.Empty);

                if (user == null)
                    return new TaskResponseErrorCode<Record?>(null, 105);


                var book = await _context.Book.AddAsync(new Book { RecordId = record.Id, ContainerId = container.Id });

                if (book.Entity == null)
                    throw new Exception("Failed to create book when creating record");

                record.Books = new List<Book>() { book.Entity };

                record.User = user;

                EntityEntry<Record> entry = _context.Record.Add(record);

                await SaveContextAsync();

                return new TaskResponseErrorCode<Record?>(entry.Entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<Record?>(null, 101);
            }
        }

        public async Task<Record?> GetAsync(int? id)
        {
            Record? result = await _context.Record.FindAsync(id);
            return result;
        }

        public async Task<Record?> SearchForIsbn(string? userId, string query)
        {
            return null;
        }

        public async Task<List<Record>> Search(string userId, string? query, string? sort = null, string? groupFilter = null)
        {
            List<Record> result = new List<Record>();

            if (string.IsNullOrWhiteSpace(query))
                query = "*";

            if (string.IsNullOrWhiteSpace(groupFilter))
                groupFilter = "*";

            string[] groupFilters = groupFilter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            List<Record> records = new List<Record>();

            if (groupFilters.Contains(userId) || groupFilter == "*")
                result.AddRange(await _context.Record.Where(x => x.UserId == userId).ToListAsync());

            var userGroup = await _context.UserGroups.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            foreach (string otherUserId in userGroup?.OtherUserIds ?? new List<string>())
            {
                if (otherUserId == userId)
                    continue;
                //result.AddRange(await context.Record.Where(x => x.UserId == userId).ToListAsync());

                if (groupFilters.Contains(otherUserId) || groupFilter == "*")
                    result.AddRange(await _context.Record.Where(x => x.UserId == otherUserId).ToListAsync());
            }

            //List<Record> personalRecords = await context.Record.Where(x => x.UserId == userId).ToListAsync();

            // Add users personal records to result
            //if (personalRecords != null)
            //result.AddRange(personalRecords);

            // Add any linked group records to result, if desired
            //if (includeGroups)
            //result.AddRange(await GetAllForUserGroups(userId));

            if (sort != null)
            {
                // Sort records
                SortRecords(ref result, sort);
            }

            // Only filter based on query if there is a supplied query string
            if (query != "*")
            {
                result = result
                    .Where(x =>
                    (x.ISBN ?? string.Empty).ToLower() == query.ToLower() ||
                    (x.Title ?? string.Empty).ToLower().Contains(query.ToLower()) ||
                    (x.Author ?? string.Empty).ToLower().Contains(query.ToLower()) ||
                    (x.Synopsis ?? string.Empty).ToLower().Contains(query.ToLower()))
                    .ToList();
            }

            return result;
        }

        private void SortRecords(ref List<Record> recordsQuery, string? sort)
        {
            if (sort == null)
                sort = string.Empty;

            switch (sort)
            {
                case "title_asc":
                    recordsQuery = recordsQuery.OrderBy(x => x.Title).ThenBy(x => x.Author).ToList();
                    break;

                case "title_desc":
                    recordsQuery = recordsQuery.OrderByDescending(x => x.Title).ThenBy(x => x.Author).ToList();
                    break;

                case "author_asc":
                    recordsQuery = recordsQuery.OrderBy(x => x.Author).ThenBy(x => x.Title).ToList();
                    break;

                case "author_desc":
                    recordsQuery = recordsQuery.OrderByDescending(x => x.Author).ThenBy(x => x.Title).ToList();
                    break;

                default:
                    recordsQuery = recordsQuery.OrderBy(x => x.Title).ToList();
                    break;
            }
        }

        private async Task<List<Record>> GetAllForUserGroups(string userId)
        {
            List<Record> result = new List<Record>();

            // Account for this user having access to other user groups
            var userGroup = await _context.UserGroups.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (userGroup != null)
            {
                foreach (string? otherUserId in userGroup.OtherUserIds ?? new List<string>())
                {
                    List<Record> groupResult = await _context.Record.Where(x => x.UserId == otherUserId).ToListAsync();

                    if (groupResult != null && groupResult.Count > 0)
                    {
                        result.AddRange(groupResult);
                    }
                }
            }

            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetSortOptions()
        {
            try
            {
                List<SelectListItem> result = new List<SelectListItem>()
                {
                    new SelectListItem("Title", "title_asc"),
                    new SelectListItem("Title (des)", "title_desc"),
                    new SelectListItem("Author", "author_asc"),
                    new SelectListItem("Author (des)", "author_desc")
                };

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to fetch sort options");
                return new List<SelectListItem>();
            }
        }

        public async Task<List<SelectListItem>> GetGroupFilterOptions(string userId)
        {
            try
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result.Add(new SelectListItem("All", "*"));

                if ((await userAccess.GetUserById(userId)) != null)
                    result.Add(new SelectListItem("You", userId));

                var userGroup = await _context.UserGroups.Where(x => x.UserId == userId).FirstOrDefaultAsync();

                foreach (var otherUserId in userGroup?.OtherUserIds ?? new List<string>())
                {
                    result.Add(new SelectListItem(await userAccess.GetUsernameByUserId(otherUserId), otherUserId));
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new List<SelectListItem>() { new SelectListItem("All", "*") };
            }
        }


    }
}
