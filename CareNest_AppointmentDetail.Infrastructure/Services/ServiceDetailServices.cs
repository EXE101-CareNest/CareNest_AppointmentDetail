using CareNest_AppointmentDetail.Application.Common;
using CareNest_AppointmentDetail.Application.Interfaces.Services;
using CareNest_AppointmentDetail.Domain.Commons.Base;
using CareNest_AppointmentDetail.Domain.Commons.Constant;
using CareNest_AppointmentDetail.Infrastructure.ApiEndpoints;
using Shared.Contracts;

namespace CareNest_AppointmentDetail.Infrastructure.Services
{
    public class ServiceDetailServices : IServiceDetailService
    {
        private readonly IAPIService _apiService;

        public ServiceDetailServices(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ResponseResult<ServiceDetailResponse>> GetServiceDetailById(string? id)
        {
            var serviceDetail = await _apiService.GetAsync<ServiceDetailResponse>("servicedetail", SerivceDetaileEndpoint.GetById(id));
            if (!serviceDetail.IsSuccess)
            {
                throw BaseException.BadRequestBadRequestResponse("ServiceDetail Id " + MessageConstant.NotFound);
            }
            return serviceDetail;
        }
    }
}
