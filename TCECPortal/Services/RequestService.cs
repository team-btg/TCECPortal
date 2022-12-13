using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TCECPortal.Models;

namespace TCECPortal.Services
{
    public class RequestService : BaseController.Base, IRequestService
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IConfiguration _configuration;
        public RequestService(ILogger<RequestService> logger, IConfiguration configuration)
        : base(logger, configuration)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _configuration = configuration;
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }
        public async Task<TResult> GetAsync<TResult>(string uri, string token = "", List<RequestHeaders> requestHeaders = null)
        {
            HttpClient httpClient = CreateHttpClient(token);
            if (requestHeaders != null)
                foreach (var reqHeader in requestHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(reqHeader.Header, reqHeader.Value);
                }
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            logApiUrlRequest(httpClient.BaseAddress + uri);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();
            TResult result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
            return result;
        }
        public Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", List<RequestHeaders> requestHeaders = null)
        {
            return PostAsync<TResult, TResult>(uri, data, token, requestHeaders);
        }
        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "", List<RequestHeaders> requestHeaders = null)
        {
            HttpClient httpClient = CreateHttpClient(token);
            if (requestHeaders != null)
                foreach (var reqHeader in requestHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(reqHeader.Header, reqHeader.Value);
                }
            logApiUrlRequest(httpClient.BaseAddress + uri);
            string serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));
            HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress + uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
            await HandleResponse(response);
            string responseData = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
        }
        private HttpClient CreateHttpClient(string token = "")
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["URi"])
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpClient;
        }
        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                    GenerateErroResponse(content, response);
            }
        }
        private ApiErrorResponse GenerateErroResponse(string content, HttpResponseMessage response)
        {
            var errorResult = new ApiErrorResponse();
            errorResult.Error = ($"API response error: <{response.ReasonPhrase}> when try get request from {response.RequestMessage.RequestUri} with method { response.RequestMessage.Method} with error message {content}");
            Logger.LogError(errorResult.Error);
            return errorResult;
        }
        public void logApiUrlRequest(string url)
        {
            Logger.LogInformation("REQUEST URL: {0}", url);
        }
        public async Task<TResult> PostTokenAuthAsync<TRequest, TResult>(string uri, TRequest data)
        {
            HttpClient httpClient = CreateHttpClient(string.Empty);
            logApiUrlRequest(httpClient.BaseAddress + uri);
            string serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));
            HttpResponseMessage response = await httpClient.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
            await HandleResponse(response);
            string responseData = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
        }
        public Task<TResult> PatchAsync<TResult>(string uri, TResult data, string token = "", List<RequestHeaders> requestHeaders = null)
        {
            return PatchAsync<TResult, TResult>(uri, data, token, requestHeaders);
        }
        public async Task<TResult> PatchAsync<TRequest, TResult>(string uri, TRequest data, string token = "", List<RequestHeaders> requestHeaders = null)
        {
            HttpClient httpClient = CreateHttpClient(token);
            if (requestHeaders != null)
                foreach (var reqHeader in requestHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(reqHeader.Header, reqHeader.Value);
                }
            logApiUrlRequest(httpClient.BaseAddress + uri);
            string serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));
            HttpResponseMessage response = await httpClient.PatchAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
            await HandleResponse(response);
            string responseData = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
        }

        public class ApiErrorResponse
        {
            public string Error { get; set; }
        }
    }
}
