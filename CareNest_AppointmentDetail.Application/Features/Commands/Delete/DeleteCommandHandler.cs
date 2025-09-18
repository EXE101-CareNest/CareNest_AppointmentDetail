using CareNest_AppointmentDetail.Application.Exceptions;
using CareNest_AppointmentDetail.Application.Interfaces.CQRS.Commands;
using CareNest_AppointmentDetail.Application.Interfaces.UOW;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Domain.Entitites;

namespace CareNest_AppointmentDetail.Application.Features.Commands.Delete
{
    public class DeleteCommandHandler : ICommandHandler<DeleteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(DeleteCommand command)
        {
            // Lấy order theo ID
            AppointmentDetail? appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetByIdAsync(command.Id)
                                              ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            _unitOfWork.GetRepository<AppointmentDetail>().Delete(appointmentDetail);

            await _unitOfWork.SaveAsync();
        }
    }
}
