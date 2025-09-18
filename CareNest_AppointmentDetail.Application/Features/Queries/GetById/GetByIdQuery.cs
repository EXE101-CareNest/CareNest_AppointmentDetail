using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<AppointmentDetail>
    {
        public required string Id { get; set; }
    }
}
