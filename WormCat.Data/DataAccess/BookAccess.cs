using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class BookAccess : IBookAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext context;
        private readonly IUserAccess userAccess;
        private readonly IContainerAccess containerAccess;
        private readonly ILocationAccess locationAccess;

        public BookAccess(ILogger<UserAccess> logger, WormCatRazorContext context, IUserAccess userAccess, IContainerAccess containerAccess, ILocationAccess locationAccess)
        {
            this.logger = logger;
            this.context = context;
            this.userAccess = userAccess;
            this.containerAccess = containerAccess;
            this.locationAccess = locationAccess;
        }

        public async Task<Book> CreateNewAsync(int recordId, int containerId)
        {
            EntityEntry<Book> book = await context.Book.AddAsync(new Book { RecordId = recordId, ContainerId = containerId });
            return book.Entity;
        }

        public async Task<Book?> CreateNewForRecordAsync(string? userId, int recordId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            // Fetch existing user
            User? user = await userAccess.GetAsync(userId);

            if (user == null) return null;

            // Fetch existing record
            Record? record = await context.Record.FindAsync(recordId);

            if (record == null) return null;

            // Fetch existing location
            Location? location = (await locationAccess.GetAllForUserAsync(userId)).FirstOrDefault();

            // If no existing locations, create one
            if (location == null)
                location = await locationAccess.CreateNewAsync(user, "Home");

            // Fetch existing container
            Container? container = await containerAccess.GetFirstOrDefaultForUserAsync(userId);

            if (container == null)
            {
                // If no existing containers, create one
                container = await containerAccess.CreateNewAsync(new Container()
                {
                    Name = "Bookshelf",
                    Location = location,
                    User = user
                });
            }

            EntityEntry<Book> bookEntity = await context.Book.AddAsync(new Book { Container = container, Record = record });

            return bookEntity.Entity;
        }

        public async Task<Book?> GetAsync(int? id)
        {
            Book? result = await context.Book.FindAsync(id);
            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
