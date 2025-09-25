using CareNest_AppointmentDetail.Application.Exceptions;
using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, AppointmentDetail>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;

        public UpdateCommandHandler(IUnitOfWork unitOfWork, IAppointmentService appointmentService)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
        }

        public async Task<AppointmentDetail> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            AppointmentDetail? appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);
            if(!string.IsNullOrWhiteSpace(command.AppointmentId))
            {
                var appointment = await _appointmentService.GetAppointmentById(command.AppointmentId);
                appointmentDetail.AppointmentId = command.AppointmentId;
            }

            appointmentDetail.Note = command.Note;
            appointmentDetail.PetQuantity = command.PetQuantity;
            appointmentDetail.ServiceDetailId = command.ServiceDetailId;
            appointmentDetail.TotalAmount = command.TotalAmount;
            appointmentDetail.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<AppointmentDetail>().Update(appointmentDetail);
            await _unitOfWork.SaveAsync();
            return appointmentDetail;

        }
    }
}
