using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaveUp.Common.Helpers;
using SaveUp.Common.Interfaces;
using SaveUp.Common.Models;
using SaveUpModels.DTOs.Responses;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SaveUp.Common
{
    /// <summary>
    /// Base class for API services covering all default CRUD operations supported by RESTful APIs
    /// Since we used generic controllers in the API we can for simplicity use a generic service on this end as well
    /// So each API controller should have a corresponding service that inherits from this class
    /// </summary>
    /// <typeparam name="TCreateRequest">Model for the create request</typeparam>
    /// <typeparam name="TUpdateRequest">Model for the update request</typeparam>
    /// <typeparam name="TResponse">Model for the response</typeparam>
    public abstract class BaseAPIService<TCreateRequest, TUpdateRequest, TResponse> : IBaseAPIService<TCreateRequest, TUpdateRequest, TResponse>
        where TCreateRequest : class
        where TUpdateRequest : class
        where TResponse : class
    {
        /// <summary>
        /// The Configuration instance used to load the base url for the API
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The individual endpoint for the API-Controller of interest
        /// </summary>
        protected readonly string _endpoint;

        public HttpClient Client => _httpClient;

        /// <summary>
        /// The HttpClient instance used to send requests to the API
        /// </summary>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Default base url for the API. Used if no value is found in appsettings.json
        /// </summary>
        /// 
        string _defaultBaseUrl =
#if ANDROID
        "http://10.0.2.2:8000/api/";
#else
        "http://localhost:8000/api/";
#endif

        /// <summary>
        /// Base url for the API. Loaded from appsettings.json or set to default value
        /// </summary>
        string _baseUrl;

        public BaseAPIService(IConfiguration configuration, string endpoint)
        {
            _configuration = configuration;
            _endpoint = endpoint;
            _httpClient = HTTPClientFactory.Create();

            var baseUrl = _configuration["API:BaseURL"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _baseUrl = baseUrl;
            }
            else
            {
                Console.WriteLine("WARNING: API:BaseURL not found in appsettings.json. Using default value: " + _defaultBaseUrl);
                _baseUrl = _defaultBaseUrl;
            }
        }

        /// <summary>
        /// Helper method to create the url for the API request
        /// </summary>
        /// <param name="parmas">Additional data</param>
        /// <returns>The BaseUrl with the enpoint. All params are added seperated by '/'</returns>
        protected string _url(params string[] parmas)
        {
            string url = _baseUrl + _endpoint;
            foreach (var param in parmas)
            {
                url += "/" + param;
            }
            return url;
        }

        /// <summary>
        /// Helper method to send a HTTP request to the API
        /// </summary>
        /// <param name="method">The method to use</param>
        /// <param name="url">The URL to send the Request to</param>
        /// <param name="data">The data to send</param>
        /// <returns>The HttpResponseMessage object</returns>
        protected async Task<HttpResponseMessage?> _sendRequest(HttpMethod method, string url, object? data = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (AuthManager.IsLoggedIn)
            {
                //request.Headers.Add("Authorization", "Bearer " + AuthManager.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthManager.Token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            try
            {
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send request to {url} - {method}");
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all entries from the API-Controller
        /// </summary>
        /// <returns>a list of the target response type</returns>
        public async Task<HTTPResponse<List<TResponse>>> GetAllAsync()
        {
            var res = await _sendRequest(HttpMethod.Get, _url());
            return new HTTPResponse<List<TResponse>>(res);
        }

        /// <summary>
        /// Get a single entry from the API-Controller
        /// </summary>
        /// <param name="id">The id of the entry to get</param>
        /// <returns>the target response type</returns>
        public async Task<HTTPResponse<TResponse>> GetAsync(int id)
        {
            var res = await _sendRequest(HttpMethod.Get, _url(id.ToString()));
            return new HTTPResponse<TResponse>(res);
        }

        /// <summary>
        /// Create a new entry in the API-Controller
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns>the target response type</returns>
        public async Task<HTTPResponse<TResponse>> CreateAsync(TCreateRequest data)
        {
            var res = await _sendRequest(HttpMethod.Post, _url(), data);
            return new HTTPResponse<TResponse>(res);
        }

        /// <summary>
        /// Update an existing entry in the API-Controller
        /// </summary>
        /// <param name="id">The id of the entry to update</param>
        /// <param name="data">The data to send</param>
        /// <returns>the target response type</returns>
        public async Task<HTTPResponse<TResponse>> UpdateAsync(int id, TUpdateRequest data)
        {
            var res = await _sendRequest(HttpMethod.Put, _url(id.ToString()), data);
            return new HTTPResponse<TResponse>(res);
        }

        /// <summary>
        /// Delete an existing entry in the API-Controller
        /// </summary>
        /// <param name="id">The id of the entry to delete</param>
        /// <returns>the target response type</returns>
        public async Task<HTTPResponse<DeleteResponse>> DeleteAsync(int id)
        {
            var res = await _sendRequest(HttpMethod.Delete, _url(id.ToString()));
            return new HTTPResponse<DeleteResponse>(res);
        }
    }
}
