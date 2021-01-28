using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient.Interfaces
{
    public interface IApiService
    {
        Uri BaseAddress { get; set; }

        JsonSerializerSettings JsonSerializer { get; set; }

        void AddHeader(string name, string value);

        void RemoveHeader(string name);

        Task<ApiServiceResponse<TResult>> GetUrl<TResult>(string apiUrl);

        Task<ApiServiceResponse<TResult>> PostUrl<TResult>(string apiUrl);

        Task<ApiServiceResponse<TResult>> PostUrl<TResult, TParam>(string apiUrl, TParam parameter);

        Task<ApiServiceResponse<TResult>> PutUrl<TResult>(string apiUrl);

        Task<ApiServiceResponse<TResult>> PutUrl<TResult, TParam>(string apiUrl, TParam parameter);

        Task<ApiServiceResponse<TResult>> DeleteUrl<TResult>(string apiUrl);

        ApiServiceResponse<T> CreateResponse<T>(T model);
    }
}
