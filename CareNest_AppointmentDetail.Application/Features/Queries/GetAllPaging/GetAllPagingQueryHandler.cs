using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<AppointmentDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PageResult<AppointmentDetailResponse>> HandleAsync(GetAllPagingQuery query)
        {
            var selector = ObjectMapperExtensions.CreateMapExpression<AppointmentDetail, AppointmentDetailResponse>();

            var orderByFunc = GetOrderByFunc(query.SortColumn, query.SortDirection);

            IEnumerable<AppointmentDetailResponse> a = await _unitOfWork.GetRepository<AppointmentDetail>().FindAsync(
                predicate: null,
                orderBy: orderByFunc,
                selector: selector,
                pageSize: query.PageSize,
                pageIndex: query.Index);

            return new PageResult<AppointmentDetailResponse>(a, 1, query.PageSize, query.Index);
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
