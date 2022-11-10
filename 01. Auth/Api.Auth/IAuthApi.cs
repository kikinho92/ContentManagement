﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Auth
{
    public interface IAuthApi
    {
        const string SERVICE_ROUTE = "auth";
        const string PING = "ping";

        /// <summary>
        /// Session information generated by the service on user login for the calling aplication to keep and use during session.
        /// </summary>
        /// <param name="jwtToken">JWT token to be included in the header of the every call to service methods (bearer authentication).</param>
        public record Session(string jwtToken, string refreshToken);

        /// <summary>
        /// Information passed by a user to initiate a new login session.
        /// </summary>
        /// <param name="userEmail">Unique ifentifier of the user attempting to initiate session</param>
        /// <param name="password">User´s secret password</param>
        public record LoginCredentials(string userEmail, string password);

        /// <summary>
        /// Information required to register a new user
        /// </summary>
        /// <param name="userEmail">Unique identifier of the new user. Also the main e-mail address</param>
        /// <param name="password">Initial password to log in this user</param>
        /// <param name="passwordConfirmed">Password to confirm that it meets the requirements</param>
        /// <param name="role">Role assigment by admin. This determinate what the user can see</param>
        /// <param name="group">Group which user belong. Usually this represent an university</param>
        public record SignUpCredentials(string userEmail, string password, string passwordConfirmed, string role, string group);

        /// <summary>
        /// Information about the user the current session is open for
        /// </summary>
        /// <param name="userId">Internal unique identifier of the user logged in</param>
        /// <param name="userEmail">Public identifier of the user logged in</param>
        /// <param name="role">Identifier of the role the user is having in this session</param>
        public record SessionInfo(string userId, string userEmail, string role);

        /// <summary>
        /// Admin is calling to register a new user providing the initial crentials
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>Error messages in case invalid credentials</returns>
        Task<string> SignUpAsync(SignUpCredentials credentials);
        const string SIGNUP_PATH = "signup";

        /// <summary>
        /// Initiates a new session for user to work in the system
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>Session´s newly generated tokens. Null if credentials are invalid.</returns>
        Task<Session> LogInAsync(LoginCredentials credentials);
        const string LOGIN_PATH = "login";

        /// <summary>
        /// Invalidates the current session so the token is no longer working.
        /// </summary>
        /// <returns>Whether the user has been actually logged out, otherwise no session was really open</returns>
        Task<bool> LogOutAsync();
        const string LOGOUT_PATH = "logout";

        /// <summary>
        /// Provides information about current session.
        /// </summary>
        /// <returns>Information about the user. Null if no session is open</returns>
        Task<SessionInfo> GetSessionInfoAsync();
        const string SESSION_PATH = "session";

        /// <summary>
        /// Provides a new session identifier in order to keep a previous login session alive. 
        /// To be called when the JWT token has just expired and the user is still actively.
        /// </summary>
        /// <param name="refreshToken">The token as received in previous login action or previous refresh action</param>
        /// <returns>Newly generated JWT token and refresh token to be used from now on. NUll if not allowed to refresh the session</returns>
        Task<Session> RefreshSessionAsync(string refreshToken);
        const string REFRESH_PATH = "refresh";

        /// <summary>
        /// Inserts the session token in the header of an htpp client library
        /// </summary>
        /// <param name="client">Object to be used for remote calls to authenticated services</param>
        void SetSession(HttpClient client);

    }
}