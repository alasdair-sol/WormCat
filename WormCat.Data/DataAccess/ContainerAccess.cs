using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class ContainerAccess : IContainerAccess
    {
        private readonly ILogger<UserAccess> logger;
        private readonly WormCatRazorContext context;

        public ContainerAccess(ILogger<UserAccess> logger, WormCatRazorContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<Container> CreateNewAsync(Container container)
        {
            var entity = await context.Container.AddAsync(container);
            return entity.Entity;
        }

        public async Task<Container?> GetFirstOrDefaultForUserAsync(string userId)
        {
            Container? result = await context.Container.Where(x => x.User.Id == userId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Container>> GetAllForUserAsync(string userId)
        {
            List<Container> result = await context.Container.Where(x => x.UserId == userId).ToListAsync();
            return result;
        }

        public async Task<Container?> GetAsync(int? id)
        {
            Container? result = await context.Container.FindAsync(id);
            return result;
        }

        public async Task<int?> SaveContextAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
