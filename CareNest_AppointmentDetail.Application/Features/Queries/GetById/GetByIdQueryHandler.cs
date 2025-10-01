using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Queries;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, AppointmentDetailResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceDetailService _serviceDetailService;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork, IServiceDetailService service)
        {
            _unitOfWork = unitOfWork;
            _serviceDetailService = service;
        }

        public async Task<AppointmentDetailResponse> HandleAsync(GetByIdQuery query)
        {
            AppointmentDetail? appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(query.Id);


            // kiểm tra service detail Id có tồn tại không
            var serviceDetail = await _serviceDetailService.GetServiceDetailById(appointmentDetail!.ServiceDetailId);

            if (appointmentDetail == null)
            {
                throw new Exception(MessageConstant.NotFound);
            }
            return new AppointmentDetailResponse
            {
                Id = appointmentDetail.Id,
                AppointmentId = appointmentDetail.AppointmentId,
                Note = appointmentDetail.Note,
                PetQuantity = appointmentDetail.PetQuantity,
                ServiceDetailId = appointmentDetail.ServiceDetailId,
                ServiceDetailName = serviceDetail.Data!.Data!.Name,
                TotalAmount = appointmentDetail.TotalAmount
            };
        }
    }
}
