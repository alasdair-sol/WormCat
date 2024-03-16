using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class LocationAccess : ILocationAccess
    {
        private readonly ILogger<LocationAccess> _logger;
        private readonly WormCatRazorContext _context;

        public LocationAccess(ILogger<LocationAccess> logger, WormCatRazorContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        //public async Task<Location?> CreateNewAsync(User? user, string locationName)
        //{
        //    if (user == null) return null;

        //    var entity = await _context.Location.AddAsync(new Location() { User = user, Name = locationName });
        //    return entity.Entity;
        //}

        public async Task<TaskResponseErrorCode<Location?>> CreateNewAsync(string? userId, Location location)
        {
            try
            {
                location.UserId = userId;
                EntityEntry<Location> entityEntry = await _context.Location.AddAsync(location);

                if (entityEntry.Entity == null)
                    return new TaskResponseErrorCode<Location?>(null, 101);

                await _context.SaveChangesAsync();

                return new TaskResponseErrorCode<Location?>(entityEntry.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<Location?>(null, 101);
            }
        }

        public async Task<TaskResponseErrorCode<bool>> DeleteLocationAsync(string? userId, int? id)
        {
            try
            {
                // Ensure a user can only delete their own root locations
                var location = await _context.Location.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (location == null)
                    return new TaskResponseErrorCode<bool>(false, 103);

                if (location.UserId != userId)
                    return new TaskResponseErrorCode<bool>(false, 104);

                var numLocations = await _context.Location.Where(x => x.UserId == userId).CountAsync();

                if (numLocations <= 1)
                    return new TaskResponseErrorCode<bool>(false, 102);

                _context.Location.Remove(location);
                await _context.SaveChangesAsync();

                return new TaskResponseErrorCode<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new TaskResponseErrorCode<bool>(false, 101);
            }
        }

        public async Task<List<Location>> GetAllForUserAsync(string userId, bool includeGroups)
        {
            List<Location> result = await _context.Location.Where(x => x.User.Id == userId).ToListAsync();

            if (includeGroups)
            {
                var userGroup = await _context.UserGroups.Where(x => x.UserId == userId).FirstOrDefaultAsync();

                foreach (var otherUserId in userGroup?.OtherUserIds ?? new List<string>())
                {
                    var otherUserLocations = await _context.Location.Where(x => x.UserId == otherUserId).ToListAsync();
                    result.AddRange(otherUserLocations);
                }
            }

            return result;
        }

        public async Task<Location?> GetAsync(int? id)
        {
            Location? result = await _context.Location.FindAsync(id);
            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
