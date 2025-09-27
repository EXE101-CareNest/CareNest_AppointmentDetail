using CareNest_NewService.Application.Common;

namespace CareNest_NewService.Application.Interfaces.Services
{
    public interface IAPIService
    {
        Task<ResponseResult<T>> GetAsync<T>(string serviceType, string url);

        Task<ResponseResult<T>> PostAsync<T>(string url, object data);

        Task<ResponseResult<T>> PutAsync<T>(string url, object data);

        Task<ResponseResult<T>> DeleteAsync<T>(string url);
    }
}
