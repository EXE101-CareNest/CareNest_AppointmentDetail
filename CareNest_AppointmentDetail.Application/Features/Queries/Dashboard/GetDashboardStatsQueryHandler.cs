using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.Dashboard
{
    public class GetDashboardStatsQueryHandler : IQueryHandler<GetDashboardStatsQuery, GetDashboardStatsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceDetailService _serviceDetailService;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, IServiceDetailService serviceDetailService)
        {
            _unitOfWork = unitOfWork;
            _serviceDetailService = serviceDetailService;
        }

        public async Task<GetDashboardStatsResponse> HandleAsync(GetDashboardStatsQuery query)
        {
            var appointmentDetails = _unitOfWork
                .GetRepository<AppointmentDetail>()
                .Entities
                .AsQueryable();

            if (query.FromDate.HasValue)
            {
                var from = new DateTimeOffset(DateTime.SpecifyKind(query.FromDate.Value, DateTimeKind.Utc));
                appointmentDetails = appointmentDetails.Where(x => x.CreatedAt >= from);
            }

            if (query.ToDate.HasValue)
            {
                var to = new DateTimeOffset(DateTime.SpecifyKind(query.ToDate.Value, DateTimeKind.Utc));
                appointmentDetails = appointmentDetails.Where(x => x.CreatedAt <= to);
            }

            var groupedByDetail = appointmentDetails
                .Where(x => x.ServiceDetailId != null)
                .GroupBy(x => x.ServiceDetailId!)
                .Select(g => new { ServiceDetailId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(query.Top)
                .ToList();

            var detailIds = groupedByDetail.Select(x => x.ServiceDetailId).ToArray();

            var batch = await _serviceDetailService.GetServiceDetailsByIds(detailIds);
            var detailMap = new Dictionary<string, (string? name, string? serviceId, string? serviceName)>();
            if (batch.IsSuccess && batch.Data != null && batch.Data.Data != null)
            {
                foreach (var item in batch.Data.Data.Items)
                {
                    if (item.Id != null)
                    {
                        detailMap[item.Id] = (item.Name, item.ServiceId, item.ServiceName);
                    }
                }
            }

            var response = new GetDashboardStatsResponse();

            response.ServiceDetailStats = groupedByDetail
                .Select(x =>
                {
                    detailMap.TryGetValue(x.ServiceDetailId, out var info);
                    return new ServiceDetailStatItem
                    {
                        ServiceDetailId = x.ServiceDetailId,
                        ServiceDetailName = info.name,
                        ServiceId = info.serviceId,
                        ServiceName = info.serviceName,
                        Count = x.Count
                    };
                })
                .ToList();

            response.ServiceStats = response.ServiceDetailStats
                .Where(x => !string.IsNullOrEmpty(x.ServiceId) || !string.IsNullOrEmpty(x.ServiceName))
                .GroupBy(x => new { x.ServiceId, x.ServiceName })
                .Select(g => new ServiceStatItem
                {
                    ServiceId = g.Key.ServiceId,
                    ServiceName = g.Key.ServiceName,
                    Count = g.Sum(i => i.Count)
                })
                .OrderByDescending(x => x.Count)
                .Take(query.Top)
                .ToList();

            return response;
        }
    }
}


