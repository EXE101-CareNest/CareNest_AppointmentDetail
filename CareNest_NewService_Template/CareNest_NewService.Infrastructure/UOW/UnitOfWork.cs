using CareNest_NewService.Application.Interfaces.UOW;
using CareNest_NewService.Infrastructure.Persistences.Database;

namespace CareNest_NewService.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
