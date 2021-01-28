using System.Net.Http;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient.Interfaces
{
    public interface IHttpStringContentBuilder
    {
        StringContent BuildContent(object body, JsonSerializerSettings jsonSerializerSettings);
    }
}
