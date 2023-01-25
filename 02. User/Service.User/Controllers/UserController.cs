using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public UserController(ILogger<UserController> logger,
                                UserDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route(PING)]
        [AllowAnonymous]
        public ActionResult<string> Ping()
        {
            return Ok("Pinged successful!");
        }

        
        [HttpGet]
        [Route(USERS_PATH)]
        public ActionResult<List<UserInfo>> GetUsers(string groupId = null)
        {
            try
            {
                List<Data.User> users = _dbContext.Users.Where(user => !string.IsNullOrEmpty(groupId) && user.GroupId == groupId).ToList();

                List<UserInfo> usersInfo = new List<UserInfo>();
                foreach (Data.User user in users)
                {
                    Group group = _dbContext.Groups.Where(group => group.Id == user.GroupId).FirstOrDefault();
                    Role role = _dbContext.Roles.Where(role => role.Id == user.RoleId).FirstOrDefault();
                    usersInfo.Add(new UserInfo( user.Id,
                                                user.IdentityId,
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
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(USER_PATH)]
        public ActionResult<UserInfo> GetUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) { return BadRequest("ERROR - Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null)
                {
                    user = _dbContext.Users.Where(user => user.IdentityId == userId).FirstOrDefault();

                    if (user == null) { return BadRequest("ERROR - Invalid user. User was not found"); }
                }

                Group group = _dbContext.Groups.Where(group => group.Id == user.GroupId).FirstOrDefault();
                Role role = _dbContext.Roles.Where(role => role.Id == user.RoleId).FirstOrDefault();
                UserInfo userInfo = new UserInfo(user.Id,
                                                user.IdentityId,
                                                user.Username,
                                                user.Email,
                                                new RoleInfo(role.Id, role.Name),
                                                new GroupInfo(group.Id, group.Name),
                                                user.CreationDate);

                return Ok(userInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(GROUPS_PATH)]
        public ActionResult<List<GroupInfo>> GetGroups()
        {
            try
            {
                List<Data.Group> groups = _dbContext.Groups.ToList();

                if (groups == null) { return BadRequest("ERROR - Invalid group. Group was not found"); }

                List<GroupInfo> groupsInfo = new List<GroupInfo>();
                foreach (Data.Group group in groups)
                {
                    groupsInfo.Add(new GroupInfo(group.Id, group.Name));
                }

                return Ok(groupsInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(GROUP_PATH)]
        public ActionResult<GroupInfo> GetGroup(string groupId)
        {
            try
            {
                if (string.IsNullOrEmpty(groupId)) { return BadRequest("ERROR - Invalid group. Group was not found"); }

                Data.Group group = _dbContext.Groups.Where(group => group.Id == groupId).FirstOrDefault();

                if (group == null) { return BadRequest("ERROR - Invalid group. Group was not found"); }

                GroupInfo groupInfo = new GroupInfo(group.Id, group.Name);

                return Ok(groupInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(GROUP_NAME)]
        public ActionResult<GroupInfo> GetGroupByName(string groupName)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName)) { return BadRequest("ERROR - Invalid group. Group was not found"); }

                Data.Group group = _dbContext.Groups.Where(group => group.Name == groupName).FirstOrDefault();

                if (group == null) { return BadRequest("ERROR - Invalid group. Group was not found"); }

                GroupInfo groupInfo = new GroupInfo(group.Id, group.Name);

                return Ok(groupInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(GROUP_PATH)]
        public ActionResult<GroupInfo> PostGroup(GroupInfo group)
        {
            try
            {
                if (group == null) { return BadRequest("ERROR - Invalid group. Group is empty"); }
                if (string.IsNullOrEmpty(group.name)) { return BadRequest("Invalid group. Name of group is empty"); }

                Group groupData = new Group() { Id = Guid.NewGuid().ToString(), Name = group.name };
                _dbContext.Groups.Add(groupData);
                _dbContext.SaveChanges();

                return Ok(group);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(GROUP_ADD)]
        public ActionResult AddToGroup(string userId, [FromBody]GroupInfo group)
        {
            try
            {
                if (group == null) { return BadRequest("ERROR - Invalid group. Group is empty"); }
                if (string.IsNullOrEmpty(group.name)) { return BadRequest("ERROR - Invalid group. Name of group is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("ERROR - Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("ERROR - Invalid user. User was not found"); }

                user.GroupId = group.id;
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(ROLES_PATH)]
        public ActionResult<GroupInfo> GetRoles(string roleId)
        {
            try
            {
                List<Data.Role> roles = _dbContext.Roles.ToList();

                if (roles == null) { return BadRequest("ERROR - Invalid role. Role was not found"); }

                List<RoleInfo> rolesInfo = new List<RoleInfo>();
                foreach (Data.Role role in roles)
                {
                    rolesInfo.Add(new RoleInfo(role.Id, role.Name));
                }

                return Ok(rolesInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(ROLE_PATH)]
        public ActionResult<GroupInfo> GetRole(string roleId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId)) { return BadRequest("ERROR - Invalid role. Role was not found"); }

                Data.Role role = _dbContext.Roles.Where(role => role.Id == roleId).FirstOrDefault();

                if (role == null) { return BadRequest("ERROR - Invalid role. Role was not found"); }

                RoleInfo roleInfo = new RoleInfo(role.Id, role.Name);

                return Ok(roleInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(ROLE_NAME)]
        public ActionResult<GroupInfo> GetRoleByName(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName)) { return BadRequest("ERROR - Invalid role. Role was not found"); }

                Data.Role role = _dbContext.Roles.Where(role => role.Name == roleName).FirstOrDefault();

                if (role == null) { return BadRequest("ERROR - Invalid role. Role was not found"); }

                RoleInfo roleInfo = new RoleInfo(role.Id, role.Name);

                return Ok(roleInfo);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(ROLE_PATH)]
        public ActionResult<RoleInfo> PostRole(RoleInfo role)
        {
            try
            {
                if (role == null) { return BadRequest("ERROR - Invalid role. Role is empty"); }
                if (string.IsNullOrEmpty(role.name)) { return BadRequest("ERROR - Invalid role. Name of role is empty"); }

                Role newRole = new Role() { Id = Guid.NewGuid().ToString(), Name = role.name };

                _dbContext.Roles.Add(newRole);
                _dbContext.SaveChanges();

                return Ok(newRole);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(ROLE_ADD)]
        public ActionResult AddToRole([FromBody] RoleInfo role, string userId)
        {
            try
            {
                if (role == null) { return BadRequest("ERROR - Invalid role. Role is empty"); }
                if (string.IsNullOrEmpty(role.name)) { return BadRequest("ERROR - Invalid role. Name of role is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("ERROR - Invalid user. User was not found"); }

                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("ERROR - Invalid user. User was not found"); }
                user.RoleId = role.id;

                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(USER_PATH)]
        public ActionResult<UserInfo> PostUser(UserInfo user)
        {
            try
            {
                if (user == null) { return BadRequest("ERROR - Invalid user. User is empty"); }
                if (string.IsNullOrEmpty(user.username)) { return BadRequest("ERROR - Invalid user. Username of user is empty"); }
                if (string.IsNullOrEmpty(user.email)) { return BadRequest("ERROR - Invalid user. Email of user is empty"); }

                Data.User userData = new Data.User()
                {
                    Id = user.id,
                    IdentityId = user.identityId,
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
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(USER_PATH)]
        public ActionResult<UserInfo> PutUser(UserInfo user, string userId)
        {
            try
            {
                if (user == null) { return BadRequest("ERROR - Invalid user. User is empty"); }
                if (string.IsNullOrEmpty(user.username)) { return BadRequest("ERROR - Invalid user. Name of user is empty"); }
                if (string.IsNullOrEmpty(user.email)) { return BadRequest("ERROR - Invalid user. Email is empty"); }
                if (string.IsNullOrEmpty(userId)) { return BadRequest("ERROR - Invalid user. User was not found"); }

                Data.User userToUpdate = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (userToUpdate == null) { return BadRequest("ERROR - Invalid user. User was not found"); }

                userToUpdate.Username = user.username;
                userToUpdate.Email = user.email;

                _dbContext.SaveChanges();

                return Ok(user);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }
        
        [HttpDelete]
        [Route(USER_PATH)]
        public ActionResult<bool> DeleteUser(string userId)
        {
            try
            {
                Data.User user = _dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

                if (user == null) { return BadRequest("ERROR - Invalid user. User was not found"); }

                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();

                return Ok(true);
            }
            catch (Exception e)
            {
                 _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }
    }
}
