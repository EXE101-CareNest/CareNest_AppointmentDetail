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
                throw BaseException.BadRequestBadRequestResponse("Appointment Id không tồn tại.");
            }
            return appointment;
        }

        public async Task<ResponseResult<AppointmentResponse>> UpdateTotalAmount(string id, double totalAmount)
        {
            // Gọi API PUT /api/appointment/{id}/total-amount với body là double
            var endpoint = AppointmentEndPoints.UpdateTotalAmount(id);
            var response = await _apiService.PutAsync<AppointmentResponse>(endpoint, totalAmount);
            return response;
        }

    }
}
