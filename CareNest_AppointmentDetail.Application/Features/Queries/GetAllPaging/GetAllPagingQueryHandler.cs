using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;
using System.Linq.Expressions;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<AppointmentDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceDetailService _detailService;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork, IServiceDetailService service)
        {
            _detailService = service;
            _unitOfWork = unitOfWork;
        }

        public async Task<PageResult<AppointmentDetailResponse>> HandleAsync(GetAllPagingQuery query)
        {
            Expression<Func<AppointmentDetail, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                predicate = ad => ad.AppointmentId.Contains(query.SearchTerm);
            }
            var selector = ObjectMapperExtensions.CreateMapExpression<AppointmentDetail, AppointmentDetailResponse>();

            var orderByFunc = GetOrderByFunc(query.SortColumn, query.SortDirection);

            IEnumerable<AppointmentDetailResponse> a = await _unitOfWork.GetRepository<AppointmentDetail>().FindAsync(
                predicate: predicate,
                orderBy: orderByFunc,
                selector: selector,
                pageSize: query.PageSize,
                pageIndex: query.Index);
            var detailList = a.ToList();
            // Load appointment details for each appointment
            foreach (var detail in detailList)
            {
                if (detail.Id != null)
                {
                    try
                    {
                        var details = await _detailService.GetServiceDetailById(detail.ServiceDetailId);
                        detail.ServiceDetailId = details.Data.Data.Id;
                        detail.ServiceDetailName = details.Data.Data.Name;
                        detail.ServiceName = details.Data.Data.ServiceName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading details for appointment {detail.Id}: {ex.Message}");
                        detail.ServiceDetailName = null;
                        detail.ServiceName = null;
                    }
                }
            }
            return new PageResult<AppointmentDetailResponse>(a, 1, query.Index, query.PageSize);
        }


        private Func<IQueryable<AppointmentDetail>, IOrderedQueryable<AppointmentDetail>> GetOrderByFunc(string? sortColumn, string? sortDirection)
        {
            var ascending = string.IsNullOrWhiteSpace(sortDirection) || sortDirection.ToLower() != "desc";

            return sortColumn?.ToLower() switch
            {
                "updateat" => q => ascending ? q.OrderBy(a => a.UpdatedAt) : q.OrderByDescending(a => a.UpdatedAt),
                _ => q => q.OrderBy(a => a.CreatedAt) // fallback nếu không có sortColumn
            };
        }
    }
}
