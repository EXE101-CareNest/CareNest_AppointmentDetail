namespace CareNest_NewService.Application.Interfaces.CQRS
{
    public interface IUseCaseDispatcher
    {
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);
        Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command);
    }
}
