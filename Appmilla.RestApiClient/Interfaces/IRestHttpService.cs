using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient.Interfaces
{
    public interface IRestHttpService
    {
        JsonSerializerSettings JsonSerializer { get; set; }

        Uri BaseAddress { get; set; }

        void AddHeader(string name, string value);

        void RemoveHeader(string name);

        void AddParameter(string key, object value);

        void AddQueryString(string key, string value);

        string GetParameters();

        string GetQueryString();

        //Task<IInputStream> GetStreamWithTimeout(Uri uri, int timeout = 30000);

        Task<HttpResponseMessage> GetWithTimeout(string uri, int timeout = 30000);

        Task<HttpResponseMessage> PostWithTimeout(string uri, int timeout = 30000);

        Task<HttpResponseMessage> PostIHttpContentWithTimeout(string uri, HttpContent content, int timeout = 30000);

        Task<HttpResponseMessage> PostJsonWithTimeout(string uri, object body, int timeout = 30000);

        Task<HttpResponseMessage> PutWithTimeout(string uri, int timeout = 30000);

        Task<HttpResponseMessage> PutJsonWithTimeout(string uri, object body, int timeout = 3000);

        Task<HttpResponseMessage> DeleteWithTimeout(string uri, int timeout = 30000);
    }
}
