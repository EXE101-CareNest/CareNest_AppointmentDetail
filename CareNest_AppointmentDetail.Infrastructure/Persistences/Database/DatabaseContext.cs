using CareNest_AppointmentDetail.Domain.Entitites;
using Microsoft.EntityFrameworkCore;


namespace CareNest_AppointmentDetail.Infrastructure.Persistences.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
