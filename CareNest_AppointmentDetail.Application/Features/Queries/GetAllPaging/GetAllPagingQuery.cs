using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;


namespace CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQuery : IQuery<PageResult<AppointmentDetailResponse>>
    {
        public int Index { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public string? SearchTerm { get; set; }
    }
}
