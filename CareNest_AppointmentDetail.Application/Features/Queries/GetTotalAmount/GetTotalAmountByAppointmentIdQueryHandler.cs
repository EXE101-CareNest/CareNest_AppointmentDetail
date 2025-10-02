using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetTotalAmount
{
    public class GetTotalAmountByAppointmentIdQueryHandler : IQueryHandler<GetTotalAmountByAppointmentIdQuery, TotalAmountResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTotalAmountByAppointmentIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TotalAmountResponse> HandleAsync(GetTotalAmountByAppointmentIdQuery query)
        {
            // Lấy tất cả appointment detail có cùng appointment id và chỉ lấy TotalAmount
            var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                .FindAsync(
                    predicate: ad => ad.AppointmentId == query.AppointmentId,
                    orderBy: null,
                    selector: ad => ad.TotalAmount
                );

            // Tính tổng tiền và số lượng
            var totalAmount = appointmentDetails.Sum();
            var count = appointmentDetails.Count();

            return new TotalAmountResponse(query.AppointmentId, totalAmount, count);
        }
    }
}