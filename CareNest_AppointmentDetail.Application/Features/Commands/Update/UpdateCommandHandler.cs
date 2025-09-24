using CareNest_AppointmentDetail.Application.Exceptions;
using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, AppointmentDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDetail> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            AppointmentDetail? appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            appointmentDetail.Note = command.Note;
            appointmentDetail.PetQuantity = command.PetQuantity;
            appointmentDetail.ServiceDetailId = command.ServiceDetailId;
            appointmentDetail.AppointmentId = command.AppointmentId;
            appointmentDetail.TotalAmount = command.TotalAmount;
            appointmentDetail.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<AppointmentDetail>().Update(appointmentDetail);
            await _unitOfWork.SaveAsync();
            return appointmentDetail;

        }
    }
}
