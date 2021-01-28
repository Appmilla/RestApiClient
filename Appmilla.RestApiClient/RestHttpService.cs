using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Appmilla.RestApiClient.Extensions;
using Appmilla.RestApiClient.Interfaces;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient
{
    // use a single instance of HttpClient see
    // http://www.nimaara.com/2016/11/01/beware-of-the-net-httpclient/
    // http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
    // http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
    // http://naeem.khedarun.co.uk/blog/2016/11/30/httpclient-dns-settings-for-azure-cloud-services-and-traffic-manager-1480285049222

    // When Asp.Net Core 2.1 is released we should hopefully be able to use the HttpClientFactory
    // This will take care of the problems documented with using shared instances of Httpclient with respect to DNS refresh
    // https://github.com/aspnet/HttpClientFactory
    // https://www.stevejgordon.co.uk/introduction-to-httpclientfactory-aspnetcore
    // https://blogs.msdn.microsoft.com/webdev/2018/02/02/asp-net-core-2-1-roadmap/#httpclientfactory
    public class RestHttpService : IRestHttpService, IDisposable
    {
        private HttpClient client;

        private bool disposed = false;
        private Dictionary<string, object> parameters;
        private Dictionary<string, object> qstring;
        //TODO set this to a sensible value like 30 seconds
        private const int timeoutInMilliseconds = 30000;

        public bool checkAccessToken = true;

        public JsonSerializerSettings JsonSerializer { get; set; }

        public Uri BaseAddress
        {
            get => client.BaseAddress;
            set => client.BaseAddress = value;
        }
       
        public RestHttpService(HttpClient httpClient)
        {
            client = httpClient;

            parameters = new Dictionary<string, object>();
            qstring = new Dictionary<string, object>();

            // setup custom Contract resolver so that serialisation honours the [IgnoreForSerialisation] attribute
            JsonSerializer = new JsonSerializerSettings
            {
                ContractResolver = new PostContractResolver()
            };

            AddRequestTimeout(15.Seconds());
        }

        private void AddRequestTimeout(TimeSpan? timeout)
        {
            if (!timeout.HasValue) { return; }
            client.Timeout = timeout.Value;
        }

        public void AddParameter(string key, object value)
        {
            parameters.Add(key, value);
        }

        public void AddQueryString(string key, string value)
        {
            qstring.Add(key, value);
        }

        public void AddHeader(string name, string value)
        {
            client.DefaultRequestHeaders.Remove(name);
            client.DefaultRequestHeaders.Add(name, value);
        }

        public void RemoveHeader(string name)
        {
            client.DefaultRequestHeaders.Remove(name);
        }

        #region Post and Get methods with server timeouts

        public async Task<HttpResponseMessage> PostWithTimeout(string uri, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(PostIHttpContentAsync(uri, null), ApiTimeoutTask(timeout, HttpMethod.Post, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostIHttpContentWithTimeout(string uri, HttpContent content, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(PostIHttpContentAsync(uri, content), ApiTimeoutTask(timeout, HttpMethod.Post, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostJsonWithTimeout(string uri, object body, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(PostJson(new HttpContentBuilder(), uri, body), ApiTimeoutTask(timeout, HttpMethod.Post, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PutWithTimeout(string uri, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(PutIHttpContentAsync(uri, null), ApiTimeoutTask(timeout, HttpMethod.Put, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PutJsonWithTimeout(string uri, object body, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(PutJson(new HttpContentBuilder(), uri, body), ApiTimeoutTask(timeout, HttpMethod.Post, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteWithTimeout(string uri, int timeout = 30000)
        {
            try
            {
                var response = await Task.WhenAny(DeleteAsync(uri), ApiTimeoutTask(timeout, HttpMethod.Delete, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> GetWithTimeout(string uri, int timeout = timeoutInMilliseconds)
        {
            try
            {
                var response = await Task.WhenAny(GetStringAsync(uri), ApiTimeoutTask(timeout, HttpMethod.Get, uri));
                return response.Result;
            }
            catch
            {
                return null;
            }
        }

        //public async Task<IInputStream> GetStreamWithTimeout(Uri uri, int timeout = timeoutInMilliseconds)
        //{
        //    try
        //    {
        //        await ApplyAccessTokenToHeader();

        //        var delayTask = Task.Delay(timeout);
        //        var streamTask = GetStreamAsync(uri);
        //        var completedTask = await Task.WhenAny(streamTask, delayTask);
        //        return streamTask.IsCompleted && !streamTask.IsFaulted ? streamTask.Result : null;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region Post and Get methods used by the timeout methods - you probably won't want to use these directly

        private async Task<HttpResponseMessage> GetStringAsync(string uri)
        {
            var response = await client.GetAsync(uri);

            return response;
        }

        private async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var response = await client.DeleteAsync(uri);

            return response;
        }

        private async Task<HttpResponseMessage> PutStringAsync(string uri, HttpContent content)
        {
            var response = await client.PutAsync(uri, content);

            return response;
        }

        //private async Task<IInputStream> GetStreamAsync(Uri uri)
        //{
        //    return await httpClient.GetInputStreamAsync(uri);
        //}

        private async Task<HttpResponseMessage> PostJson<T>(T contentBuilder, string uri, object body)
            where T : IHttpStringContentBuilder
        {
            HttpRequestMessage msg =
                new HttpRequestMessage(new HttpMethod("POST"), uri)
                {
                    Content = contentBuilder.BuildContent(body, JsonSerializer)
                };

            msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(msg);

            return response;
        }

        private async Task<HttpResponseMessage> PostIHttpContentAsync(string uri, HttpContent content)
        {
            var response = await client.PostAsync(uri, content);

            return response;
        }

        private async Task<HttpResponseMessage> PutIHttpContentAsync(string uri, HttpContent content)
        {
            var response = await client.PutAsync(uri, content);

            return response;
        }

        private async Task<HttpResponseMessage> PutJson<T>(T contentBuilder, string uri, object body)
            where T : IHttpStringContentBuilder
        {
            HttpRequestMessage msg =
                new HttpRequestMessage(new HttpMethod("PUT"), uri)
                {
                    Content = contentBuilder.BuildContent(body, JsonSerializer)
                };

            msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // for debugging
            //string msgAsString = await msg.Content.ReadAsStringAsync();

            var response = await client.SendAsync(msg);

            return response;
        }

        #endregion

        private async Task<HttpResponseMessage> ApiTimeoutTask(int timeout, HttpMethod httpMethod, string uri)
        {
            await Task.Delay(timeout);

            HttpResponseMessage message =
                new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    RequestMessage = new HttpRequestMessage(httpMethod, uri),
                    ReasonPhrase = "Connection timeout while waiting for server to respond.",
                    StatusCode = HttpStatusCode.RequestTimeout
                };

            return message;
        }

        public string GetParameters()
        {
            if (!parameters.Any())
                return "";

            var builder = new StringBuilder();

            foreach (var kvp in parameters.Where(kvp => kvp.Value != null))
            {
                //builder.AppendFormat("{0}", WebUtility.UrlEncode(kvp.Value));
                builder.AppendFormat("{0}", kvp.Value);
                builder.Append("/");

            }

            return builder.ToString();
        }

        public string GetQueryString()
        {
            if (!qstring.Any())
                return "";

            var builder = new StringBuilder("?");

            foreach (var kvp in qstring.Where(kvp => kvp.Value != null))
            {
                //builder.AppendFormat("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value));
                builder.AppendFormat("{0}={1}", WebUtility.UrlEncode(kvp.Key), kvp.Value);
                builder.Append(("&"));
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        #region Disposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                client?.Dispose();
                client = null;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion
    }
}
