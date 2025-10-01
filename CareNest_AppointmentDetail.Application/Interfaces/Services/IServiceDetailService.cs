using CareNest_AppointmentDetail.Application.Common;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Application.Interfaces.Services
{
    public interface IServiceDetailService
    {
        Task<ResponseResult<ServiceDetailResponse>> GetServiceDetailById(string? id);

    }
}
