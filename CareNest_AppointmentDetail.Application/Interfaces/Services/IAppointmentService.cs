using CareNest_AppointmentDetail.Application.Common;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<ResponseResult<AppointmentResponse>> GetAppointmentById(string? id);
    }
}
