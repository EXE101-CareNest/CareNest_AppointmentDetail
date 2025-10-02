using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<AppointmentDetailResponse>
    {
        public required string Id { get; set; }
    }
}
