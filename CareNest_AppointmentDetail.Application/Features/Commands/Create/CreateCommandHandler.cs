using CareNest_AppointmentDetail.Application.Exceptions.Validators;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, AppointmentDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDetail> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            AppointmentDetail appointmentDetail = new()
            {
                Note = command.Note,
                AppointmentId = command.AppointmentId,
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
