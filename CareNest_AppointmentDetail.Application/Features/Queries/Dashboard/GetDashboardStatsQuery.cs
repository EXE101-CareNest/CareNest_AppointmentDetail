using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;

namespace CareNest_AppointmentDetail.Application.Features.Queries.Dashboard
{
    public class GetDashboardStatsQuery : IQuery<GetDashboardStatsResponse>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Top { get; set; } = 10;
        public string? AppointmentId { get; set; }
    }
}


