namespace CareNest_NewService.Application.Interfaces.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
    }
}
