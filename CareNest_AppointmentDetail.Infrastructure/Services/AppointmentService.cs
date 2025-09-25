using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Domain.Commons.Base;
using CareNest_AppointmentDetail.Infrastructure.Common.ApiEndpoints;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAPIService _apiService;

        public AppointmentService(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ResponseResult<AppointmentResponse>> GetAppointmentById(string? id)
        {
            var appointment = await _apiService.GetAsync<AppointmentResponse>("appointment", AppointmentEndPoints.GetById(id));
            if (!appointment.IsSuccess)
            {
                throw BaseException.BadRequestBadRequestResponse("Mã qui trình không tồn tại.");
            }
            return appointment;
        }

    }
}
