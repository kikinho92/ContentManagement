using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Auth;
using Api.Content;
using static Api.Content.IContentApi;

namespace Sdk.Content
{
    public class ContentCli : IContentApi 
    {
        /// <summary>
        /// Creates a client tool for accessing the web service
        /// located in the specified URL.
        /// </summary>
        /// <param name="serviceBaseUrl">Base URL where the web service is serving</param>
        /// <param name="authCli">Auth service client library. Required to include authentication header</param>
        public ContentCli(string serviceBaseUrl, IAuthApi authCli)
        {
            _base = serviceBaseUrl;
            _client = new HttpClient();
            _authCli = authCli;
        }

        /// <summary>
        /// Creates a client tool for accessing the web service
        /// for automatic tests purposes. The HttpClient object is passed
        /// so that the integration test cases can make use of the testing
        /// client tool instead of the actual one.
        /// </summary>
        /// <param name="testClient">Alternative HttpClient tool to be used</param>
        /// <param name="authCli">Auth service client library. Required to include authentication header</param>
        public ContentCli(HttpClient testClient, IAuthApi authCli)
        {
            _base = "";
            _client = testClient;
            _authCli = authCli;
        }

        public async Task<List<ContentInfo>> GetContents(int pageSize, int page, string tagId = null, string groupId = null)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{CONTENT_LIST_PATH}"+
                    $"?{PAGE_SIZE}={pageSize}/" +
                    $"&{PAGE}={page}" +
                    $"&{TAG_ID_PATH}={tagId}/" +
                    $"&{GROUP_ID_PATH}={groupId}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<List<ContentInfo>>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<ContentInfo> GetContent(string contentId)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{CONTENT_PATH}" +
                    $"?{CONTENT_ID_PATH}={contentId}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<ContentInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<ContentInfo> PostContent(ContentInfo content)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<ContentInfo>(content), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{CONTENT_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<ContentInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<ContentInfo> PutContent(ContentInfo content, string contentId)
        {
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<ContentInfo>(content), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{CONTENT_PATH}" +
                    $"?{CONTENT_ID_PATH}={contentId}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<ContentInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> DeleteContent(string contentId)
        {
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.DeleteAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{CONTENT_PATH}" +
                    $"?{CONTENT_ID_PATH}={contentId}");

            // Provide success.
            if (response.IsSuccessStatusCode){ return true; }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<List<TagInfo>> GetTags(string groupId = null)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{TAG_PATH}"+
                    $"?{GROUP_ID_PATH}={groupId}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<List<TagInfo>>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<TagInfo> PostTag(TagInfo tag)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<TagInfo>(tag), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IContentApi.SERVICE_ROUTE}/" +
                    $"{TAG_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<TagInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Overrides the session token as provided by the auth client by using the passed one.
        /// This is used by calls from service to service transferring the session from call to call.
        /// </summary>
        public void UseSession(string jwtToken)
        {
            _jwtTokenOverridden = jwtToken;
        }

        /// <summary>
        /// Places in the request header for the next remote calls the authentication
        /// tokens with the help of the Auth SDK that holds the current session tokens,
        /// or using the overridden one if present.
        /// </summary>
        private void ArrangeAuthenticatedRequest()
        {
            // Overridden session JWT token.
            if (_jwtTokenOverridden != null) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtTokenOverridden);

            // Using the session as provided by the inyected auth service client.
            if (_authCli == null) return;
            _authCli.SetSession(_client);
        }
    
        private HttpClient _client;
        private string _base;
        private IAuthApi _authCli;
        private string _jwtTokenOverridden;

    }
}
