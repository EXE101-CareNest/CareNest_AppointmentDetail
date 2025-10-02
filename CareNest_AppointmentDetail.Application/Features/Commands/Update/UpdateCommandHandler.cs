using CareNest_AppointmentDetail.Application.Exceptions;
using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Features.Queries.GetAllPaging;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, AppointmentDetailResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceDetailService _serviceDetailService;

        public UpdateCommandHandler(IUnitOfWork unitOfWork, IAppointmentService appointmentService, IServiceDetailService serviceDetailService)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
            _serviceDetailService = serviceDetailService;
        }

        public async Task<AppointmentDetailResponse> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            AppointmentDetail? appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);


            var appointment = await _appointmentService.GetAppointmentById(command.AppointmentId);
            appointmentDetail.AppointmentId = appointment.Data!.Data!.Id;


            var serviceDetail = await _serviceDetailService.GetServiceDetailById(command.ServiceDetailId);
            appointmentDetail.ServiceDetailId = serviceDetail.Data!.Data!.Id;

            appointmentDetail.Note = command.Note;
            appointmentDetail.PetQuantity = command.PetQuantity;
            appointmentDetail.ServiceDetailId = command.ServiceDetailId;
            appointmentDetail.TotalAmount = serviceDetail.Data!.Data!.Price * command.PetQuantity;
            appointmentDetail.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<AppointmentDetail>().Update(appointmentDetail);
            await _unitOfWork.SaveAsync();

            // Sau khi cập nhật, tính lại tổng tiền cho appointment
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
                ServiceDetailName = serviceDetail.Data!.Data!.Name,
                TotalAmount = appointmentDetail.TotalAmount
            };
        }
    }
}
