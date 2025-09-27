using CareNest_NewService.Application.Interfaces.CQRS;
using CareNest_NewService.Application.Interfaces.CQRS.Commands;
using CareNest_NewService.Application.Interfaces.CQRS.Queries;
using MediatR;

namespace CareNest_NewService.Application.UseCases
{
    public class UseCaseDispatcher : IUseCaseDispatcher
    {
        private readonly IMediator _mediator;

        public UseCaseDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
        {
            return await _mediator.Send(query);
        }

        public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            return await _mediator.Send(command);
        }
    }
}
