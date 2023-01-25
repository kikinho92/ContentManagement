using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.User
{
    public interface IUserApi
    {
        const string SERVICE_ROUTE = "user";
        const string PING = "ping";

        public record UserInfo(string id, string identityId, string username, string email, RoleInfo role, GroupInfo group, DateTime creationDate);
        public record RoleInfo(string id, string name);
        public record GroupInfo(string id, string name);
        
        /// <summary>
        /// Provides detailed information about all the users. In case of groupId is not null, it will be provide the user that belong to the group.
        /// </summary>
        /// <param name="groupId">Unique internal identifier of the group</param>
        Task<List<UserInfo>> GetUsers(string groupId = null);
        const string USERS_PATH = "users";
        const string GROUP_ID_PATH = "groupId";
        
        /// <summary>
        /// Provides detailed information about the user requested
        /// </summary>
        /// <param name="userId">Unique internal identifier of the user</param>
        Task<UserInfo> GetUser(string userId);
        const string USER_PATH = "user";
		const string USER_ID_PATH = "userId";

        /// <summary>
        /// Provides detailed information about the groups in the system
        /// </summary>
        Task<List<GroupInfo>> GetGroups();
        const string GROUPS_PATH = "groups";

        /// <summary>
        /// Provides detailed information about the user group requested
        /// </summary>
        /// <param name="groupId">Unique internal identifier of the group</param>
        Task<GroupInfo> GetGroup(string groupId);
        const string GROUP_PATH = "group";

        /// <summary>
        /// Provides detailed information about the user group requested
        /// </summary>
        /// <param name="groupName">Unique internal identifier name of the group</param>
        Task<GroupInfo> GetGroupByName(string groupName);
        const string GROUP_NAME = "groupName";

        /// <summary>
        /// Save detailed information about group
        /// </summary>
        /// <param name="group">Group entity that contains the information</param>
        Task<GroupInfo> PostGroup(GroupInfo group);

        /// <summary>
        /// Assign user to a given group
        /// </summary>
        /// <param name="userId">Unique internal identifier of the user</param>
        /// <param name="group">Group entity that contains the information</param>
        Task AddToGroup(GroupInfo group, string userId);
        const string GROUP_ADD = "addToGroup";

        /// <summary>
        /// Provides the detailed information about the roles in the system
        /// </summary>
        /// <returns></returns>
        Task<List<RoleInfo>> GetRoles();
        const string ROLES_PATH = "roles";

        /// <summary>
        /// Provides the detailed information about the role requested
        /// </summary>
        /// <param name="roleId">Unique internal identifier of the role</param>
        /// <returns></returns>
        Task<RoleInfo> GetRole(string roleId);
        const string ROLE_PATH = "role";
        const string ROLE_ID_PATH = "roleId";

        /// <summary>
        /// Provides the detailed information about the role requested
        /// </summary>
        /// <param name="roleName">Unique internal name identifier of the role</param>
        /// <returns></returns>
        Task<RoleInfo> GetRoleByName(string roleName);
        const string ROLE_NAME = "roleName";

        /// <summary>
        /// Save detailed information about role
        /// </summary>
        /// <param name="role">Role entity that contains the information</param>
        Task<RoleInfo> PostRole(RoleInfo role);
         
        /// <summary>
        /// Assign user to a given role
        /// </summary>
        /// <param name="userId">Unique internal identifier of the user</param>
        /// <param name="role">Role entity that contains the information</param>
        Task AddToRole(RoleInfo role, string userId);
        const string ROLE_ADD = "addToRole";
        
        /// <summary>
        /// Save detailed information about user
        /// </summary>
        /// <param name="user">User entity that contains the information</param>
        Task<UserInfo> PostUser(UserInfo user);

        /// <summary>
        /// Modifies user who already exist in the db
        /// </summary>
        /// <param name="user">User entity that contains the new information</param>
        /// <param name="userId">Unique internal identifier of the user who is going to be replaced</param>
        Task<UserInfo> PutUser(UserInfo user, string userId);
        
        /// <summary>
        /// Removes from the system the information about user.
        /// </summary>
        /// <param name="userId">Unique internal identifier of the user to be deleted</param>
        Task<bool> DeleteUser(string userId);
    }
}
