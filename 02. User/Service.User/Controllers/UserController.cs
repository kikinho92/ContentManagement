using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.User.Data;
using static Api.User.IUserApi;

namespace Service.User.Controllers
{
    [ApiController]
    [Route(SERVICE_ROUTE)]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UserDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
         private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ILogger<UserController> logger,
                                UserDbContext dbContext,
                                UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route(PING)]
        [AllowAnonymous]
        public ActionResult<string> Ping()
        {
            return Ok("Pinged successful!");
        }

        
        [HttpGet]
        [Route(USERS_PATH + "/{" + GROUP_ID_PATH + "}")]
        public async Task<ActionResult<List<UserInfo>>> GetUsers(string groupId = null)
        {
            try
            {
                List<Data.User> users = _dbContext.Users.Where(user => !string.IsNullOrEmpty(groupId) && user.GroupId == groupId).ToList();

                List<UserInfo> usersInfo = new List<UserInfo>();
                foreach (Data.User user in users)
                {
                    Group group = _dbContext.Groups.Where(group => group.Id == user.GroupId).FirstOrDefault();
                    IdentityRole role = await _roleManager.FindByIdAsync(user.RoleId);
                    usersInfo.Add(new UserInfo( user.Id,
                                                user.Username,
                                                user.Email,
                                                new RoleInfo(role.Id, role.Name), 
                                                new GroupInfo(group.Id, group.Name), 
                                                user.CreationDate));
                }

                return Ok(usersInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(USERS_PATH + "/{" + USER_ID_PATH + "}")]
        public async Task<ActionResult<UserInfo>> GetUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) { return BadRequest("Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("Invalid user. User was not found"); }

                Group group = _dbContext.Groups.Where(group => group.Id == user.GroupId).FirstOrDefault();
                IdentityRole role = await _roleManager.FindByIdAsync(user.RoleId);
                UserInfo userInfo = new UserInfo(user.Id,
                                                user.Username,
                                                user.Email,
                                                new RoleInfo(role.Id, role.Name),
                                                new GroupInfo(group.Id, group.Name),
                                                user.CreationDate);

                return Ok(userInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(GROUP_PATH + "/{" + GROUP_ID_PATH + "}")]
        public ActionResult<GroupInfo> GetGroup(string groupId)
        {
            try
            {
                if (string.IsNullOrEmpty(groupId)) { return BadRequest("Invalid group. Group was not found"); }

                Data.Group group = _dbContext.Groups.Where(group => group.Id == groupId).FirstOrDefault();

                if (group == null) { return BadRequest("Invalid group. Group was not found"); }

                GroupInfo groupInfo = new GroupInfo(group.Id, group.Name);

                return Ok(groupInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(GROUP_NAME + "/{" + GROUP_ID_PATH + "}")]
        public ActionResult<GroupInfo> GetGroupByName(string groupName)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName)) { return BadRequest("Invalid group. Group was not found"); }

                Data.Group group = _dbContext.Groups.Where(group => group.Name == groupName).FirstOrDefault();

                if (group == null) { return BadRequest("Invalid group. Group was not found"); }

                GroupInfo groupInfo = new GroupInfo(group.Id, group.Name);

                return Ok(groupInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(GROUP_PATH)]
        public ActionResult<GroupInfo> PostGroup(GroupInfo group)
        {
            try
            {
                if (group == null) { return BadRequest("Invalid group. Group is empty"); }
                if (string.IsNullOrEmpty(group.name)) { return BadRequest("Invalid group. Name of group is empty"); }

                Group groupData = new Group() { Id = Guid.NewGuid().ToString(), Name = group.name };
                _dbContext.Groups.Add(groupData);
                _dbContext.SaveChanges();

                return Ok(group);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(GROUP_ADD + "/{" + USER_ID_PATH + "}")]
        public ActionResult AddToGroup(string userId, GroupInfo group)
        {
            try
            {
                if (group == null) { return BadRequest("Invalid group. Group is empty"); }
                if (string.IsNullOrEmpty(group.name)) { return BadRequest("Invalid group. Name of group is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("Invalid user. User was not found"); }

                user.GroupId = group.id;
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(ROLE_PATH)]
        public async Task<ActionResult<RoleInfo>> PostRole(RoleInfo role)
        {
            try
            {
                if (role == null) { return BadRequest("Invalid role. Role is empty"); }
                if (string.IsNullOrEmpty(role.name)) { return BadRequest("Invalid role. Name of role is empty"); }

                IdentityRole identityRole = new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = role.name };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (!result.Succeeded) { return BadRequest("Invalid role. Role can not be stored"); }

                return Ok(role);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(ROLE_ADD + "/{" + USER_ID_PATH + "}")]
        public async Task<ActionResult> AddToRole(string userId, RoleInfo role)
        {
            try
            {
                if (role == null) { return BadRequest("Invalid role. Role is empty"); }
                if (string.IsNullOrEmpty(role.name)) { return BadRequest("Invalid role. Name of role is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("Invalid user. User was not found"); }

                IdentityUser identityUser = await _userManager.FindByEmailAsync(user.Email);
                IdentityResult result = await _userManager.AddToRoleAsync(identityUser, role.name);

                if (!result.Succeeded) { return BadRequest("Invalid role. User can not be assign to the role"); }

                return Ok();
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(USER_PATH)]
        public ActionResult<UserInfo> PostUser(UserInfo user)
        {
            try
            {
                if (user == null) { return BadRequest("Invalid user. User is empty"); }
                if (string.IsNullOrEmpty(user.username)) { return BadRequest("Invalid user. Username of user is empty"); }
                if (string.IsNullOrEmpty(user.email)) { return BadRequest("Invalid user. Email of user is empty"); }

                Data.User userData = new Data.User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = user.username,
                    Email = user.email,
                    RoleId = null,
                    GroupId = null,
                    CreationDate = DateTime.Now
                };
                _dbContext.Users.Add(userData);
                _dbContext.SaveChanges();

                return Ok(user);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(USER_PATH + "/{" + USER_ID_PATH + "}")]
        public async Task<ActionResult<UserInfo>> PutUser(UserInfo user, string userId)
        {
            try
            {
                if (user == null) { return BadRequest("Invalid user. User is empty"); }
                if (string.IsNullOrEmpty(user.username)) { return BadRequest("Invalid user. Name of user is empty"); }
                if (string.IsNullOrEmpty(user.email)) { return BadRequest("Invalid user. Email is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("Invalid user. User was not found"); }

                Data.User userData = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (userData == null) { return BadRequest("Invalid user. User was not found"); }

                IdentityUser identityUser = await _userManager.FindByEmailAsync(userData.Email);
                identityUser.UserName = user.username;
                identityUser.Email = user.email;
                IdentityResult result = await _userManager.UpdateAsync(identityUser);

                if (!result.Succeeded) { return BadRequest("User can not be delete"); }

                userData.Username = user.username;
                userData.Email = user.email;
                _dbContext.SaveChanges();

                return Ok(user);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }
        
        [HttpDelete]
        [Route(USER_PATH + "/{" + USER_ID_PATH + "}")]
        public async Task<ActionResult<bool>> DeleteUser(string userId)
        {
            try
            {
                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("Invalid user. User was not found"); }

                IdentityUser identityUser = await _userManager.FindByEmailAsync(user.Email);
                IdentityResult result = await _userManager.DeleteAsync(identityUser);

                if (!result.Succeeded) { return BadRequest("User can not be delete"); }

                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();

                return Ok(true);
            }
            catch (Exception e)
            {
                 _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }
    }
}
