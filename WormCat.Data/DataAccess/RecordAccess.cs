using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class RecordAccess : IRecordAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext context;
        private readonly IBookAccess bookAccess;
        private readonly IContainerAccess containerAccess;
        private readonly IUserAccess userAccess;

        public RecordAccess(ILogger<UserAccess> logger, WormCatRazorContext context, IBookAccess bookAccess, IContainerAccess containerAccess, IUserAccess userAccess)
        {
            this.logger = logger;
            this.context = context;
            this.bookAccess = bookAccess;
            this.containerAccess = containerAccess;
            this.userAccess = userAccess;
        }

        public async Task<Record?> CreateNewAsync(string userId, Record record)
        {
            var user = await userAccess.GetAsync(userId);

            if (user == null)
            {
                logger.LogError($"No user {userId} exists in database");
                return null;
            }

            var container = await containerAccess.GetFirstOrDefaultForUserAsync(userId);

            if (container == null)
            {
                logger.LogError("No containers exist in database");
                return null;
            }



            var book = await bookAccess.CreateNewAsync(record.Id, container.Id);

            if (book == null)
                return null;

            record.Books = new List<Book>() { book };
            record.User = user;

            EntityEntry<Record> entry = context.Record.Add(record);

            return entry.Entity;
        }

        public async Task<Record?> GetAsync(int? id)
        {
            Record? result = await context.Record.FindAsync(id);
            return result;
        }

        public async Task<List<Record>> GetAllForUserAsync(string userId, bool includeGroups = false)
        {
            List<Record> result = new List<Record>();

            List<Record> personalResult = await context.Record.Where(x => x.UserId == userId).ToListAsync();

            result.AddRange(personalResult);

            if (includeGroups)
            {
                // Account for this user having access to other user groups
                var userGroup = await context.UserGroups.Where(x => x.UserId == userId).FirstOrDefaultAsync();

                if (userGroup != null)
                {
                    foreach (string? otherUserId in userGroup.OtherUserIds ?? new List<string>())
                    {
                        List<Record> groupResult = await context.Record.Where(x => x.UserId == otherUserId).ToListAsync();

                        if (groupResult != null && groupResult.Count > 0)
                        {
                            result.AddRange(groupResult);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
