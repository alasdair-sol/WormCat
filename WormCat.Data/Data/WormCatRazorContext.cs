using Microsoft.EntityFrameworkCore;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.Data
{
    public class WormCatRazorContext : DbContext
    {
        public WormCatRazorContext(DbContextOptions<WormCatRazorContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<UserGroup> UserGroups { get; set; } = default!;
        public DbSet<UserGroupInvite> UserGroupInvite { get; set; } = default!;
        public DbSet<Record> Record { get; set; } = default!;
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Container> Container { get; set; } = default!;
        public DbSet<Location> Location { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Containers)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(e => e.UserId)
            //    .IsRequired();

        }
    }
}
