using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Appmilla.RestApiClient.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> ToObject<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JObject.Parse(content).ToObject<T>();
        }

        public static async Task<T> ToArrayObject<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JArray.Parse(content).ToObject<T>();
        }
    }
}
