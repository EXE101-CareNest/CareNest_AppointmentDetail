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

        public async Task<ResponseResult<ServiceDetailBatchResponse>> GetServiceDetailsByIds(IEnumerable<string> ids, IEnumerable<string>? fields = null, int timeoutSeconds = 5)
        {
            var requestBody = new
            {
                ids = ids?.Distinct().ToArray() ?? Array.Empty<string>(),
                fields = (fields == null || !fields.Any())
                    ? new[] { "id", "name", "serviceId", "serviceName" }
                    : fields.ToArray()
            };

            string baseUrl = _apiService.GetBaseUrl("servicedetail");
            string endpoint = $"{baseUrl}/api/servicedetail/by-ids";

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var response = await _apiService.PostAsync<ServiceDetailBatchResponse>(endpoint, requestBody);

            if (!response.IsSuccess)
            {
                // Trả về rỗng thay vì throw để dashboard có thể tiếp tục render phần count
                return new ResponseResult<ServiceDetailBatchResponse>
                {
                    IsSuccess = false,
                    Message = response.Message,
                    ErrorCode = response.ErrorCode,
                    Data = new ApiResponse<ServiceDetailBatchResponse>(false, response.Message, new ServiceDetailBatchResponse())
                };
            }

            return response;
        }
    }
}
