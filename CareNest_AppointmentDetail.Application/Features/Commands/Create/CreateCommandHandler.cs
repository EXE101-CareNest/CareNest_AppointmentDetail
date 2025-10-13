using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, AppointmentDetailResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceDetailService _serviceDetailService;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IAppointmentService appointmentService, IServiceDetailService serviceDetailService)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
            _serviceDetailService = serviceDetailService;
        }

        public async Task<AppointmentDetailResponse> HandleAsync(CreateCommand command)
        {
            // valid dữ liệu đầu vào 
            //Validate.ValidateCreate(command);

            // kiểm tra appointmentId có tồn tại không
            var appointment = await _appointmentService.GetAppointmentById(command.AppointmentId);

            // kiểm tra service detail Id có tồn tại không
            var serviceDetail = await _serviceDetailService.GetServiceDetailById(command.ServiceDetailId);

            var unitPrice = serviceDetail.Data!.Data!.Price ?? 0;
            var totalPrice = unitPrice * command.PetQuantity;
            AppointmentDetail appointmentDetail = new()
            {
                Note = command.Note,
                AppointmentId = appointment.Data!.Data!.Id,
                PetQuantity = command.PetQuantity,
                ServiceDetailId = serviceDetail.Data!.Data!.Id,
                ServiceId = serviceDetail.Data!.Data!.ServiceId,
                ServiceName = serviceDetail.Data!.Data!.ServiceName,
                ServiceDetailName = serviceDetail.Data!.Data!.Name,
                TotalAmount = totalPrice,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<AppointmentDetail>().AddAsync(appointmentDetail);
            await _unitOfWork.SaveAsync();

            // Sau khi tạo mới, tính lại tổng tiền cho appointment
            // Lấy tất cả appointment detail của appointmentId này
            var allDetails = await _unitOfWork.GetRepository<AppointmentDetail>().FindAsync(ad => ad.AppointmentId == appointmentDetail.AppointmentId);
            var totalAmount = allDetails.Sum(ad => ad.TotalAmount);
            await _appointmentService.UpdateTotalAmount(appointmentDetail.AppointmentId!, totalAmount);

            return new AppointmentDetailResponse
            {
                Id = appointmentDetail.Id,
                AppointmentId = appointmentDetail.AppointmentId,
                Note = appointmentDetail.Note,
                PetQuantity = appointmentDetail.PetQuantity,
                ServiceDetailId = appointmentDetail.ServiceDetailId,
                ServiceDetailName = appointmentDetail.ServiceDetailName,
                ServiceName = appointmentDetail.ServiceName,
                TotalAmount = appointmentDetail.TotalAmount
            };
        }
    }
}
