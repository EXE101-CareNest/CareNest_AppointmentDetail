
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Delete
{
    public class DeleteCommand : ICommand
    {
        public required string Id { get; set; }
    }
}
