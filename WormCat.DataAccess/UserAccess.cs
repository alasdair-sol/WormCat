using System.Data.Entity;
using WormCat.Data.Data;
using WormCat.Library.Models.Dbo;

namespace WormCat.DataAccess
{
    public class UserAccess : IUserAccess
    {
        private readonly WormCatRazorContext context;

        public UserAccess(WormCatRazorContext context)
        {
            this.context = context;
        }

        public async Task<User> Create(int id)
        {
            var result = await context.Users.AddAsync(new User() { Id = id });
            return result.Entity;
        }

        public async Task<User> Get(int id)
        {
            User result = await context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            return result;
        }
    }
}
