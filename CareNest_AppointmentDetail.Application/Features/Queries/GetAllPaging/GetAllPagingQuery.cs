using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;


namespace CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQuery : IQuery<PageResult<AppointmentDetailResponse>>
    {
        public int Index { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; } // "Name", "Note", "CreatedAt"
        public string? SortDirection { get; set; } // "asc" or "desc"
    }
}
