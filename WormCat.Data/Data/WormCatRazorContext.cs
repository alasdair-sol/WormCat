using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models;

namespace WormCat.Data.Data
{
    public class WormCatRazorContext : DbContext
    {
        public WormCatRazorContext(DbContextOptions<WormCatRazorContext> options)
            : base(options)
        {

        }

        public DbSet<Record> Record { get; set; } = default!;
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Container> Container { get; set; } = default!;
        public DbSet<Location> Location { get; set; } = default!;
    }
}
