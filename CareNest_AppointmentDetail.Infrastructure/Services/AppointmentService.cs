using CareNest_AppointmentDetail.Application.Interfaces.Services;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Infrastructure.Services
{
    public class AppointmentService: IAppointmentService
    {
         private readonly IAPIService _apiService;

        public AppointmentService(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public async Task<AppointmentResponse>  GetAppointmentById (string id)
        {


        }

    }
}
