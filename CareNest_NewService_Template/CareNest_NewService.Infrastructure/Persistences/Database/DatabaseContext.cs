using CareNest_NewService.Domain.Entitites;
using Microsoft.EntityFrameworkCore;

namespace CareNest_NewService.Infrastructure.Persistences.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // Thêm DbSet cho các entity của bạn
        // public DbSet<YourEntity> YourEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các entity ở đây
            // modelBuilder.ApplyConfiguration(new YourEntityConfiguration());
        }
    }
}
