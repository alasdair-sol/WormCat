using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class BookAccess : IBookAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext _context;
        private readonly IUserAccess userAccess;
        private readonly IContainerAccess containerAccess;
        private readonly ILocationAccess locationAccess;
        private readonly IRecordAccess _recordAccess;

        public BookAccess(ILogger<UserAccess> logger, WormCatRazorContext context, IUserAccess userAccess, IContainerAccess containerAccess, 
            ILocationAccess locationAccess, IRecordAccess recordAccess)
        {
            this.logger = logger;
            this._context = context;
            this.userAccess = userAccess;
            this.containerAccess = containerAccess;
            this.locationAccess = locationAccess;
            _recordAccess = recordAccess;
        }

        public async Task<Book> CreateNewAsync(int recordId, int containerId)
        {
            EntityEntry<Book> book = await _context.Book.AddAsync(new Book { RecordId = recordId, ContainerId = containerId });
            return book.Entity;
        }

        public async Task<Book?> CreateNewForRecordAsync(string? userId, int recordId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            // Fetch existing user
            User? user = await userAccess.GetUserById(userId);

            if (user == null) return null;

            // Fetch existing record
            Record? record = await _context.Record.FindAsync(recordId);

            if (record == null) return null;

            // Fetch existing location
            Location? location = (await locationAccess.GetAllForUserAsync(userId, false)).FirstOrDefault();

            // If no existing locations, create one
            if (location == null)
                location = (await locationAccess.CreateNewAsync(user.Id, new Location() { Name = "Home" })).Result;

            if (location == null)
                return null;

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

            EntityEntry<Book> bookEntity = await _context.Book.AddAsync(new Book { Container = container, Record = record });

            return bookEntity.Entity;
        }

        public async Task<Book?> GetAsync(int? id)
        {
            Book? result = await _context.Book.FindAsync(id);
            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<TaskResponseErrorCode<bool>> UpdateBookAsync(Book book)
        {
            try
            {
                if (book == null)
                    throw new ArgumentNullException(nameof(book));

                Book? previousData = await _context.Book.Where(x => x.Id == book.Id).AsNoTracking().FirstOrDefaultAsync();

                if (previousData == null)
                    throw new NullReferenceException(nameof(book));

                if (previousData.Record == null)
                    throw new NullReferenceException(nameof(book));

                string previousContainerUserId = previousData.Container?.UserId
                    ?? throw new Exception("Previous container owner does not exist");

                Container? newContainer = await containerAccess.GetAsync(book.ContainerId);

                string newContainerUserId = newContainer?.UserId
                    ?? throw new Exception("New container owner does not exist");

                if(previousContainerUserId != newContainerUserId)
                {
                    // Check if new owner has a data entry for this book's -record-

                    Record? existingRecord = await _context.Record.Where(x => x.ISBN == previousData.Record.ISBN && x.UserId == newContainerUserId).FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        book.Record = existingRecord;
                        book.RecordId = existingRecord.Id;
                    }
                    else
                    {
                        Record recordToCreate = new Record()
                        {
                            ISBN = previousData.Record.ISBN,
                            Title = previousData.Record.Title,
                            Author = previousData.Record.Author,
                            Image = previousData.Record.Image,
                            PageCount = previousData.Record.PageCount,
                            PublicationDate = previousData.Record.PublicationDate,
                            Synopsis = previousData.Record.Synopsis
                        };

                        TaskResponseErrorCode<Record?> recordCreateResponse = await _recordAccess.CreateNewAsyncWithoutDefaultBook(newContainerUserId, recordToCreate);

                        if (recordCreateResponse.Result == null)
                            throw new Exception("Failed to create new user record when transferring book ownership");
                        else
                        {
                            book.Record = recordCreateResponse.Result;
                            book.RecordId = recordCreateResponse.Result.Id;
                        }
                    }

                }

                _context.Book.Attach(book).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    return new TaskResponseErrorCode<bool>(true);
                }
                catch
                {
                    if (_context.Book.Any(e => e.Id == book.Id) == false)
                    {
                        throw new NullReferenceException(nameof(book));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<bool>(false, 101);
            }
        }
    }
}
