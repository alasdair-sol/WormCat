using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class UserAccess : IUserAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext context;

        public UserAccess(ILogger<UserAccess> logger, WormCatRazorContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<User?> TryCreateNewAsync(string userId)
        {
            var existingUser = await GetAsync(userId);

            if (existingUser != null)
                return existingUser;

            EntityEntry<User> user = await context.Users.AddAsync(new User()
            {
                Id = userId
            });

            EntityEntry<Library.Models.Dbo.Location> location = await context.Location.AddAsync(new Library.Models.Dbo.Location()
            {
                Name = "Home",
                User = user.Entity
            });

            EntityEntry<Container> container = await context.Container.AddAsync(new Container()
            {
                Name = "Bookshelf",
                Location = location.Entity,
                User = user.Entity
            });

            await context.SaveChangesAsync();

            if (user.Entity != null)
                logger.LogInformation($"Created UserDbo in WormCatRazorContext with id {userId}");
            else return null;

            return user.Entity;
        }

        public async Task<User?> GetAsync(string id)
        {
            User? result = await context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            return result;
        }
    }
}
