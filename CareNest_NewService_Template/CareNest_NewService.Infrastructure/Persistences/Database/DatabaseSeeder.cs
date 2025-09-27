namespace CareNest_NewService.Infrastructure.Persistences.Database
{
    public class DatabaseSeeder
    {
        private readonly DatabaseContext _context;

        public DatabaseSeeder(DatabaseContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Thêm logic seed data ở đây
            await _context.SaveChangesAsync();
        }
    }
}
