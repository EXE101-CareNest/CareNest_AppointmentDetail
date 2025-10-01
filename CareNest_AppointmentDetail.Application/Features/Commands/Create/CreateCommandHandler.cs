using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, AppointmentDetail>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IAppointmentService appointmentService)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
        }

        public async Task<AppointmentDetail> HandleAsync(CreateCommand command)
        {
            // valid dữ liệu đầu vào 
            Validate.ValidateCreate(command);
            // kiểm tra appointmentId có tồn tại không
            
            var appointment = await _appointmentService.GetAppointmentById(command.AppointmentId);
            AppointmentDetail appointmentDetail = new()
            {
                Note = command.Note,
                AppointmentId = appointment.Data!.Data!.Id,
                PetQuantity = command.PetQuantity,
                ServiceDetailId = command.ServiceDetailId,
                TotalAmount = command.TotalAmount,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<AppointmentDetail>().AddAsync(appointmentDetail);
            await _unitOfWork.SaveAsync();

            return appointmentDetail;
        }
    }
}
