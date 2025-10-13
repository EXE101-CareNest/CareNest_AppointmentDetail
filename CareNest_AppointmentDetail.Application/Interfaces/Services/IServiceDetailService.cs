using CareNest_AppointmentDetail.Application.Common;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Application.Interfaces.Services
{
    public interface IServiceDetailService
    {
        Task<ResponseResult<ServiceDetailResponse>> GetServiceDetailById(string? id);
        Task<ResponseResult<ServiceDetailBatchResponse>> GetServiceDetailsByIds(IEnumerable<string> ids, IEnumerable<string>? fields = null, int timeoutSeconds = 5);

    }
}
