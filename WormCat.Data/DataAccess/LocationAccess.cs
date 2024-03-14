using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class LocationAccess : ILocationAccess
    {
        private readonly ILogger<LocationAccess> logger;
        private readonly WormCatRazorContext context;

        public LocationAccess(ILogger<LocationAccess> logger, WormCatRazorContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<Location?> CreateNewAsync(User? user, string locationName)
        {
            if (user == null) return null;

            var entity = await context.Location.AddAsync(new Location() { User = user, Name = locationName });
            return entity.Entity;
        }

        public async Task<List<Location>> GetAllForUserAsync(string userId)
        {
            List<Location> result = await context.Location.Where(x => x.User.Id == userId).ToListAsync();
            return result;
        }

        public async Task<Location?> GetAsync(int? id)
        {
            Location? result = await context.Location.FindAsync(id);
            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
