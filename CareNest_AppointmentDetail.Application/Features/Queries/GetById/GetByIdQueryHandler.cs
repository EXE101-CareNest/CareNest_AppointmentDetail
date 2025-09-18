using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, AppointmentDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDetail> HandleAsync(GetByIdQuery query)
        {
            AppointmentDetail? appointment = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(query.Id);

            if (appointment == null)
            {
                throw new Exception(MessageConstant.NotFound);
            }
            return appointment;
        }
    }
}
