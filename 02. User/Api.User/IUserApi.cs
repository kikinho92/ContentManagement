using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.User
{
    public interface IUserApi
    {
        const string SERVICE_ROUTE = "user";
        const string PING = "ping";

        public record UserInfo(string id, string username, string email, RoleInfo role, GroupInfo group, DateTime creationDate);
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
        /// Provides detailed information about the user group requested
        /// </summary>
        /// <param name="groupId">Unique internal identifier of the group</param>
        Task<GroupInfo> GetGroup(string groupId);
        const string GROUP_PATH = "group";

        /// <summary>
        /// Provides detailed information about the user group requested
        /// </summary>
        /// <param name="groupName">Unique internal identifier of the group</param>
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
        Task AddToGroup(string userId, GroupInfo group);
        const string GROUP_ADD = "addToGroup";

        /// <summary>
        /// Save detailed information about role
        /// </summary>
        /// <param name="role">Role entity that contains the information</param>
        Task<RoleInfo> PostRole(RoleInfo role);
         const string ROLE_PATH = "role";

        /// <summary>
        /// Assign user to a given role
        /// </summary>
        /// <param name="userId">Unique internal identifier of the user</param>
        /// <param name="role">Role entity that contains the information</param>
        Task AddToRole(string userId, RoleInfo role);
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
