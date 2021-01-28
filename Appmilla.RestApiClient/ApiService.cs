using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Appmilla.RestApiClient.Extensions;
using Appmilla.RestApiClient.Interfaces;
using Appmilla.RestApiClient.Logging;
using Appmilla.RestApiClient.Logging.Interfaces;
using Newtonsoft.Json;

namespace Appmilla.RestApiClient
{
    public class ApiService : IApiService
    {
        private readonly ILoggingService loggingService;
        private readonly IRestHttpService httpService;

        private JsonSerializerSettings jsonSerializer;

        public Uri BaseAddress
        {
            get => httpService.BaseAddress;

            set => httpService.BaseAddress = value;
        }

        public JsonSerializerSettings JsonSerializer
        {
            get => jsonSerializer;
            set
            {
                jsonSerializer = value;

                httpService.JsonSerializer = value;
            }
        }
        
        public ApiService(ILoggingService loggingService, HttpClient httpClient)
         : this(loggingService, new RestHttpService(httpClient))
        {
        }

        private ApiService(ILoggingService loggingService, IRestHttpService httpService)
        {
            this.loggingService = loggingService;
            this.httpService = httpService;

            JsonSerializer = new JsonSerializerSettings
            {
                ContractResolver = new HttpGetContractResolver()
            };

            httpService.JsonSerializer = JsonSerializer;
        }

        public void AddHeader(string name, string value)
        {
            httpService.AddHeader(name, value);
        }

        public void RemoveHeader(string name)
        {
            httpService.RemoveHeader(name);
        }

        public async Task<ApiServiceResponse<TResult>> GetUrl<TResult>(string apiUrl)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.GetWithTimeout(apiUrl).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }
       
        public async Task<ApiServiceResponse<TResult>> PostUrl<TResult>(string apiUrl)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.PostWithTimeout(apiUrl).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }

        public async Task<ApiServiceResponse<TResult>> PostUrl<TResult, TParam>(string apiUrl, TParam parameter)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.PostJsonWithTimeout(apiUrl, parameter).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }

        public async Task<ApiServiceResponse<TResult>> PutUrl<TResult>(string apiUrl)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.PutWithTimeout(apiUrl).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }

        public async Task<ApiServiceResponse<TResult>> PutUrl<TResult, TParam>(string apiUrl, TParam parameter)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.PutJsonWithTimeout(apiUrl, parameter).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }

        public async Task<ApiServiceResponse<TResult>> DeleteUrl<TResult>(string apiUrl)
        {
            var responseModel = new ApiServiceResponse<TResult>();

            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, apiUrl);

            var response = await httpService.DeleteWithTimeout(apiUrl).ConfigureAwait(false);

            responseModel = await CreateResponse<TResult>(response).ConfigureAwait(false);

            return responseModel;
        }

        private async Task<ApiServiceResponse<T>> CreateResponse<T>(HttpResponseMessage response)
        {
            var responseModel = new ApiServiceResponse<T>();

            try
            {
                if (response == null)
                {
                    responseModel.ResponseType = ResponseTypes.ConnectionError;
                    responseModel.Error.ErrorText = "Cannot establish a connection with the server";
                }
                else if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    responseModel.ResponseType = ResponseTypes.Success;

                    if (typeof(T) != typeof(PostResponseModel))
                    {
                        //var responseResult = await response.ToObject<T>();
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var responseResult = JsonConvert.DeserializeObject<T>(content, JsonSerializer);
                        responseModel.Result = responseResult;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NoContent)// this was previously HttpStatusCode.None not sure if this is correct
                {
                    responseModel.ResponseType = ResponseTypes.Cancelled;
                    responseModel.Error.HttpStatusCode = response.StatusCode;
                    responseModel.Error.ErrorText = response.Content.ToString();
                }
                else
                {
                    responseModel.ResponseType = ResponseTypes.HttpError;
                    responseModel.Error.HttpStatusCode = response.StatusCode;
                    responseModel.Error.ErrorText = response.Content.ToString();
                }
            }
            catch (Exception ex)
            {
                responseModel.ResponseType = ResponseTypes.DeserialisationError;
                responseModel.Error.HttpStatusCode = response.StatusCode;
                responseModel.Error.ErrorText = $"Failed to deserialise : {ex.Message}";
            }

            return responseModel;
        }

        public ApiServiceResponse<T> CreateResponse<T>(T model)
        {
            var response = new ApiServiceResponse<T>
            {
                ResponseType = ResponseTypes.Success,
                Result = model
            };

            return response;
        }
    }
}
