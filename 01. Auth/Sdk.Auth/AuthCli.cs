using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Auth;
using static Api.Auth.IAuthApi;

namespace Sdk.Auth
{
    public class AuthCli : IAuthApi
    {

        /// <summary>
        /// Creates a client tool for accessing the web service located in the specified URL.
        /// </summary>
        /// <param name="serviceBaseUrl">Base URL where the web service is serving</param>
        public AuthCli(string serviceBaseUrl)
        {
            _base = serviceBaseUrl;
            _client = new HttpClient();
        }

        /// <summary>
        /// Creates a client tool for accessing the web service
        /// for automatic tests purposes. The HttpClient object is passed
        /// so that the integration test cases can make use of the testing
        /// client tool instead of the actual one.
        /// </summary>
        /// <param name="testClient">Alternative HttpClient tool to be used</param>
        public AuthCli(HttpClient testClient)
        {
            _base = "";
            _client = testClient;
        }

        public async Task<string> SignUpAsync(SignUpCredentials credentials)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<SignUpCredentials>(credentials), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IAuthApi.SERVICE_ROUTE}/" +
                    $"{SIGNUP_PATH}/", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsStringAsync();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<Session> LogInAsync(LoginCredentials credentials)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<LoginCredentials>(credentials), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_base}/" +
                    $"{IAuthApi.SERVICE_ROUTE}/" +
                    $"{LOGIN_PATH}/", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                Session newSession = await response.Content.ReadAsAsync<Session>();

                // Keep session information in this object to use in following authenticated calls.
                _currentSession = new Session(newSession.jwtToken, newSession.refreshToken);
                return newSession;
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> LogOutAsync()
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.DeleteAsync($"{_base}/" +
                    $"{IAuthApi.SERVICE_ROUTE}/" +
                    $"{LOGOUT_PATH}/");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                _currentSession = null; // No longer logged in

                if (response.StatusCode == HttpStatusCode.NoContent) return false;
                return await response.Content.ReadAsAsync<bool>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<SessionInfo> GetSessionInfoAsync()
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IAuthApi.SERVICE_ROUTE}/" +
                    $"{SESSION_PATH}/");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadAsAsync<SessionInfo>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<Session> RefreshSessionAsync(string refreshToken)
        {
            // Send request.
            ArrangeAuthenticatedRequest();
            StringContent data = new StringContent(JsonSerializer.Serialize<string>(refreshToken), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IAuthApi.SERVICE_ROUTE}/" +
                    $"{REFRESH_PATH}/", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                Session refreshedSession = await response.Content.ReadAsAsync<Session>();

                // Keep session information in this object to use in following authenticated calls.
                _currentSession = new Session(refreshedSession.jwtToken, refreshedSession.refreshToken);
                return refreshedSession;
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Places the session JWT token of the currently open session in the header of the passed client library instance.
        /// This way, the following calls to the corresponding service will be authorized appropriately.
        /// To be used by other services client libraries just before every authorized requests.
        /// </summary>
        /// <param name="client"></param>
        public void SetSession(HttpClient client)
        {
            if(_currentSession != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _currentSession.jwtToken);
            else client.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// Configures the http client library to include the authentication JWT token in the header of all follingw calls.
        /// </summary>
        private void ArrangeAuthenticatedRequest()
        {
            if(_currentSession != null) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _currentSession.jwtToken);
            else _client.DefaultRequestHeaders.Authorization = null;
        }

        private HttpClient _client;
        private string _base;
        private Session _currentSession;
    }
}
