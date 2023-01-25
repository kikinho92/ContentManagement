using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Api.Auth;
using Api.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Service.Auth.Data;
using Service.User.Data;
using static Api.Auth.IAuthApi;
using static Api.User.IUserApi;

namespace Service.Auth.Controllers
{
    /// <summary>
    /// The Auth service provides the way to register new user credentials.
    /// Also provides the way to manage roles and permissions
    /// The auth service is the centralized place where other services chech the authorization
    /// tokens everytime they received an external call
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(IAuthApi.SERVICE_ROUTE)]
    public class AuthController : ControllerBase
    {

        public static readonly int PASSWORD_MIN_LENGTH = 8;
        public static readonly int USER_LOCKEDOUT_MINUTES = 5;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AuthDbContext _dbContext;
        private readonly IUserApi _user;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> signInManager,
                                AuthDbContext dbContext,
                                IUserApi user,
                                ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _user = user;
            _logger = logger;
        }

        [HttpGet]
        [Route(IAuthApi.PING)]
        [AllowAnonymous]
        public ActionResult<string> Ping()
        {
            return Ok("Pinged successful!");
        }

        [HttpPost]
        [Route(SIGNUP_PATH)]
        [AllowAnonymous]
        public async Task<ActionResult<string>> SignUpAsync(SignUpCredentials credentials)
        {
            if (credentials == null) return BadRequest("Invalid credentials: credentials were not found");
            if (string.IsNullOrEmpty(credentials.userEmail)) return BadRequest("Invalid credentials: missing user email address");
            if (string.IsNullOrEmpty(credentials.password)) return BadRequest("Invalid credentials: missing user password");

            IdentityResult result = await _userManager.CreateAsync(new IdentityUser()
            {
                UserName = credentials.userEmail,
                Email = credentials.userEmail
            }, credentials.password);

            if (!result.Succeeded)
            {
                string errorMessage = "Password not accepted: ";
                if(result.Errors.FirstOrDefault(e => e.Code == "PasswordRequireDigit") != null) errorMessage += "Valid passwords required at least one digit";
                if(result.Errors.FirstOrDefault(e => e.Code == "PasswordRequireLowercase") != null) errorMessage += "Valid passwords required at least one lower case letter";
                if(result.Errors.FirstOrDefault(e => e.Code == "PasswordRequireUppercase") != null) errorMessage += "Valid passwords required at least one upper case letter/n";
                if(result.Errors.FirstOrDefault(e => e.Code == "PasswordRequireNonAlphanumeric") != null) errorMessage += "Valid passwords required at least one non alphanumeric character";
                if(result.Errors.FirstOrDefault(e => e.Code == "PasswordTooShort") != null) errorMessage += $"Valid passwords required at least {PASSWORD_MIN_LENGTH} different characters";
                
                if(result.Errors.FirstOrDefault(e => e.Code == "DuplicateUserName") != null) errorMessage += "Invalid user name. User already exists";
                if(result.Errors.FirstOrDefault(e => e.Code == "DuplicateEmail") != null) errorMessage += "Invalid user email. User already exists";

                return BadRequest(errorMessage);
            }

            IdentityUser user = (await _userManager.FindByEmailAsync(credentials.userEmail));

            UserInfo userToCreate = new UserInfo(
                                            Guid.NewGuid().ToString(),
                                            user.Id,
                                            user.NormalizedUserName,
                                            user.NormalizedEmail,
                                            null,
                                            null,
                                            DateTime.Now);
            await _user.PostUser(userToCreate);

            RoleInfo role = await _user.GetRoleByName(credentials.role);
            if (role == null)
            {
                role = new RoleInfo(Guid.NewGuid().ToString(), credentials.group);
                await _user.PostRole(role);
            }
            GroupInfo group = await _user.GetGroupByName(credentials.group);
            if (group == null)
            {
                group = new GroupInfo(Guid.NewGuid().ToString(), credentials.group);
                await _user.PostGroup(group);
            }

            await _user.AddToRole(role, userToCreate.id);
            await _user.AddToGroup(group, userToCreate.id);

            return Ok(null);
        }

        [HttpPost]
        [Route(LOGIN_PATH)]
        [AllowAnonymous]
        public async Task<ActionResult<Session>> LogInAsync(LoginCredentials credentials)
        {
            if (credentials == null) return BadRequest("ERROR - Invalid credentials: credentials were not found");
            if (string.IsNullOrEmpty(credentials.userEmail)) return BadRequest("ERROR - Invalid credentials: missing user email address");
            if (string.IsNullOrEmpty(credentials.password)) return BadRequest("ERROR - Invalid credentials: missing user password");

            //Check if already logged in
            string currentUserId = this.HttpContext.User?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if(currentUserId != null) return BadRequest("Already logged in");

            IdentityUser user = await _userManager.FindByEmailAsync(credentials.userEmail);
            if (user == null)
            {
                _logger.LogWarning($"Attempt to log in with invalid user e-mail '{credentials.userEmail}'");
                WaitRoom();
                return Ok(null);
            }

            //Check password
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, credentials.password, false, true);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Attempt to log in with invalid password by '{credentials.userEmail}' {(result.IsLockedOut ? "LOCKEDOUT" : "")}");

                if (result.IsLockedOut)
                {
                    return Unauthorized($"User {credentials.userEmail} is locked out for {USER_LOCKEDOUT_MINUTES} minutes due to too many consecutive failed login attepmts");
                }else
                {
                    WaitRoom();
                    return Ok(null);
                }
            }

            UserInfo userInfo = await _user.GetUser(user.Id);

            UserSession userSession = new UserSession()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                UserEmail = user.Email,
                UserRole = userInfo.role.name,
                RefreshToken = Guid.NewGuid().ToString(),
                OpenTime = DateTime.Now,
            };
            _dbContext.UserSession.Add(userSession);
            _dbContext.SaveChanges();

            Session session = new Session(
                JwtHelper.GenerateJwtToken(userSession.UserId, userSession.UserEmail, userSession.UserRole),
                userSession.RefreshToken
            );

            _logger.LogInformation($"OPENNED session for user '{userSession.UserEmail}'");
            return Ok(session);
        }

        [HttpDelete]
        [Route(LOGOUT_PATH)]
        public ActionResult<bool> LogOutAsync()
        {
            string userEmail = this.HttpContext.User?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null) return Ok(false);

            // Using refresh token to identify the session to be closed
            string refreshTokenInHeader = JwtHelper.ValidateAndExtractJwtTokenRefreshToken(this.HttpContext);

            // Take the user session from database
            UserSession userSession = _dbContext.UserSession.Where(us => us.UserEmail == userEmail &&
                                                                         us.RefreshToken == refreshTokenInHeader).FirstOrDefault();

            // No session stored in database for current JWT/Refresh tokens
            if (userSession == null) return Ok(false);

            // This session was already closed
            if (userSession.CloseTime != null) return Ok(false);

            // Closing session
            userSession.CloseTime = DateTime.Now;
            _dbContext.SaveChanges();

            _logger.LogInformation($"CLOSED session for '{userSession.UserEmail}'");
            return Ok(true);

        }

        [HttpGet]
        [Route(SESSION_PATH)]
        [AllowAnonymous]
        public ActionResult<SessionInfo> GetSessionInfoAsync()
        {
            // Get current logged in user
            SessionInfo session = JwtHelper.ValidateAndExtractJwtTokenInfo(this.HttpContext);

            if (session == null) return Ok(null);

            return Ok(session);
        }

        [HttpPut]
        [Route(REFRESH_PATH)]
        [AllowAnonymous]
        public ActionResult<Session> RefreshSessionAsync([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Unable to refresh session token. Missing refresh token.");

            // The old JWT token is expected to still be present in request header
            HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
            if (token.FirstOrDefault() == null) return BadRequest("Unable to ref resh session token. Missing expired JWT token.");

            // Take stored session. The session is allowed to be refreshed if last refresh was not too long ago
            DateTime now = DateTime.Now;
            DateTime expiration = now.AddMinutes(-1 * JwtHelper.JWT_EXPIRATION_MINUTES);
            UserSession userSession = _dbContext.UserSession.Where(us => us.RefreshToken == refreshToken &&
                                                                    us.CloseTime == null &&
                                                                    (us.OpenTime >= expiration || (us.LastRefresh != null &&
                                                                                                   us.LastRefresh >= expiration))).OrderByDescending(us => us.OpenTime).FirstOrDefault();

            if (userSession == null) return Ok(null);

            userSession.RefreshToken = Guid.NewGuid().ToString();
            userSession.LastRefresh = now;
            _dbContext.SaveChanges();

            Session sessionToken = new Session(
                JwtHelper.GenerateJwtToken(userSession.UserId, userSession.UserEmail, userSession.UserRole),
                userSession.RefreshToken
            );

            _logger.LogInformation($"REFRESED session for user '{userSession.UserEmail}'");
            return (sessionToken);
        }

        private void WaitRoom()
        {
            Thread.Sleep(1000 + new Random().Next(2000));
        }
    }
}
