using BlazorBattles.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorBattles.Server.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Unit>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<User>().Property(p => p.Id).UseIdentityAlwaysColumn();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
