namespace CareNest_NewService.Application.Interfaces.CQRS.Commands
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}
