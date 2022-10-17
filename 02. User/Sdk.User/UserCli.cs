using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Auth;
using Api.User;
using static Api.User.IUserApi;

namespace Sdk.User
{
    public class UserCli : IUserApi
    {
        /// <summary>
        /// Creates a client tool for accessing the web service
        /// located in the specified URL.
        /// </summary>
        /// <param name="serviceBaseUrl">Base URL where the web service is serving</param>
        /// <param name="authCli">Auth service client library. Required to include authentication header</param>
        public UserCli(string serviceBaseUrl, IAuthApi authCli)
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
        public UserCli(HttpClient testClient, IAuthApi authCli)
        {
            _base = "";
            _client = testClient;
            _authCli = authCli;
        }

        public async Task<List<UserInfo>> GetUsers(string groupId)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{USERS_PATH}/"+
                    $"{groupId}/");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<List<UserInfo>>();
            }

            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo> GetUser(string userId)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{USER_PATH}/" +
                    $"{userId}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<UserInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<GroupInfo> GetGroup(string groupId)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{GROUP_PATH}/" +
                    $"{GROUP_ID_PATH}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<GroupInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<GroupInfo> GetGroupByName(string groupId)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{GROUP_PATH}/" +
                    $"{GROUP_NAME}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<GroupInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<GroupInfo> PostGroup(GroupInfo group)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<GroupInfo>(group), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{GROUP_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<GroupInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task AddToGroup(string userId, GroupInfo group)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<GroupInfo>(group), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{GROUP_ADD}/" +
                    $"{USER_ID_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode){ return; }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<RoleInfo> PostRole(RoleInfo role)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<RoleInfo>(role), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{ROLE_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<RoleInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

         public async Task AddToRole(string userId, RoleInfo role)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<RoleInfo>(role), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{ROLE_ADD}/" +
                    $"{USER_ID_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode){ return; }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo> PostUser(UserInfo user)
        {
           // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<UserInfo>(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{USER_PATH}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<UserInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo> PutUser(UserInfo user, string userId)
        {
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<UserInfo>(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{USER_PATH}/" +
                    $"{userId}", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<UserInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> DeleteUser(string userId)
        {
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.DeleteAsync($"{_base}/" +
                    $"{IUserApi.SERVICE_ROUTE}/" +
                    $"{USER_PATH}/" +
                    $"{userId}");

            // Provide success.
            if (response.IsSuccessStatusCode){ return true; }
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
