namespace CareNest_AppointmentDetail.Application.Features.Queries.Dashboard
{
    public class GetDashboardStatsResponse
    {
        public List<ServiceDetailStatItem> ServiceDetailStats { get; set; } = new List<ServiceDetailStatItem>();
        public List<ServiceStatItem> ServiceStats { get; set; } = new List<ServiceStatItem>();
    }

    public class ServiceDetailStatItem
    {
        public string? ServiceDetailId { get; set; }
        public string? ServiceDetailName { get; set; }
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int Count { get; set; }
    }

    public class ServiceStatItem
    {
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int Count { get; set; }
    }
}


