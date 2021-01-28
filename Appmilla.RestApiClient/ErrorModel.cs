using System.Net;
using Appmilla.RestApiClient.Interfaces;

namespace Appmilla.RestApiClient
{
    public class ErrorModel : IEntity
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string ErrorText { get; set; }
    }
}
