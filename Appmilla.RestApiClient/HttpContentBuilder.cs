using System.Net.Http;
using Appmilla.RestApiClient.Interfaces;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient
{
    public class HttpContentBuilder : IHttpStringContentBuilder
    {
        public StringContent BuildContent(object body, JsonSerializerSettings jsonSerializerSettings)
        {
            string val = JsonConvert.SerializeObject(body, jsonSerializerSettings);

            return new StringContent(JsonConvert.SerializeObject(body, jsonSerializerSettings));
        }
    }
}
