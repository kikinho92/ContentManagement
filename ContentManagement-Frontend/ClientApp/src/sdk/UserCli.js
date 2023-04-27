import AuthCli from "./AuthCli"
/**
 * JavaScript version of the User service API
 */
export default class UserCli {

  //
  // Path constants
  //
  static get SERVICE_ROUTE() { return "user" }
  static get USERS_PATH() { return "users" }
  static get GROUP_ID_PATH() { return "groupId" }
  static get USER_PATH() { return "user" }
  static get USER_ID_PATH() { return "userId" }
  static get GROUPS_PATH() { return "groups" }
  static get GROUP_PATH() { return "group" }
  static get GROUP_NAME() { return "groupName" }
  static get GROUP_ADD() { return "addToGroup" }
  static get ROLES_PATH() { return "roles" }
  static get ROLE_PATH() { return "role" }
  static get ROLE_ADD() { return "addToRole" }
  static get ROLE_ID_PATH() { return "roleId" }
  static get ROLE_NAME() { return "groupName" }

  static get prefix() {return ('http://localhost:8002/')}
  /**
   * Provides detailed information about all the users. In case of groupId is not null, it will be provide the user that belong to the group.
   * @param {string} groupId Unique internal identifier of the group
   * @returns 
   */
  static async GetUsers(groupId){
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.USERS_PATH + "?" + this.GROUP_ID_PATH + "=" + groupId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }
  
  /**
   * Provides detailed information about the user requested
   * @param {string} userId Unique internal identifier of the user
   * @returns 
   */
  static async GetUser(userId){
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.USER_PATH + "?" + this.USER_ID_PATH + "=" + userId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Provides detailed information about the groups in the system
   * @returns 
   */
  static async GetGroups(){
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.GROUPS_PATH
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Provides detailed information about the user group requested
   * @param {string} groupId Unique internal identifier of the group
   * @returns 
   */
  static async GetGroup(groupId){
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.GROUP_PATH + "?" + this.GROUP_ID_PATH + "=" + groupId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Provides detailed information about the user group requested
   * @param {string} groupName Unique internal identifier name of the group
   * @returns 
   */
  static async GetGroupByName(groupName){
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.GROUP_PATH + "?" + this.GROUP_NAME + "=" + groupName
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Save detailed information about group
   * @param {GroupInfo} group Group entity that contains the information
   * @returns 
   */
  static async PostGroup(group){
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.GROUP_PATH
    var body = group
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Assign user to a given group
   * @param {string} userId Unique internal identifier of the user
   * @param {GroupInfo} group Group entity that contains the information
   * @returns 
   */
  static async AddToGroup(userId, group){
    var verb = "PUT"
    var path = this.SERVICE_ROUTE + "/" + this.GROUP_ADD + "?" + this.USER_ID_PATH + "=" + userId
    var body = group
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

    /**
   * Provides detailed information about the roles in the system
   * @returns 
   */
    static async GetRoles(){
      var verb = "GET"
      var path = this.SERVICE_ROUTE + "/" + this.ROLES_PATH
      var body = null
    
      return AuthCli.Fetch(this.prefix + path, verb, body)
    }
  
    /**
     * Provides detailed information about the role requested
     * @param {string} roleId Unique internal identifier of the role
     * @returns 
     */
    static async GetRole(roleId){
      var verb = "GET"
      var path = this.SERVICE_ROUTE + "/" + this.ROLE_PATH + "?" + this.ROLE_ID_PATH + "=" + roleId
      var body = null
    
      return AuthCli.Fetch(this.prefix + path, verb, body)
    }
  
    /**
     * Provides detailed information about the role requested
     * @param {string} groupName Unique internal identifier name of the role
     * @returns 
     */
    static async GetRoleByName(roleName){
      var verb = "GET"
      var path = this.SERVICE_ROUTE + "/" + this.ROLE_PATH + "?" + this.ROLE_NAME + "=" + roleName
      var body = null
    
      return AuthCli.Fetch(this.prefix + path, verb, body)
    }

  /**
   * Save detailed information about role
   * @param {RoleInfo} role Role entity that contains the information
   * @returns 
   */
  static async PostRole(role){
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.ROLE_PATH
    var body = role
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Assign user to a given role
   * @param {*} userId Unique internal identifier of the user
   * @param {*} role Role entity that contains the information
   * @returns 
   */
  static async AddToRole(userId, role){
    var verb = "PUT"
    var path = this.SERVICE_ROUTE + "/" + this.ROLE_ADD + "?" + this.USER_ID_PATH + "=" + userId
    var body = role
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Save detailed information about user
   * @param {UserInfo} user User entity that contains the information
   * @returns 
   */
  static async PostUser(user){
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.USER_PATH
    var body = user
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Modifies user who already exist in the db
   * @param {UserInfo} user User entity that contains the new information
   * @param {string} userId Unique internal identifier of the user who is going to be replaced
   * @returns 
   */
  static async PutUser(user, userId){
    var verb = "PUT"
    var path = this.SERVICE_ROUTE + "/" + this.USER_PATH + "?" + this.USER_ID_PATH + "=" + userId
    var body = user
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Removes from the system the information about user
   * @param {string} userId Unique internal identifier of the user to be deleted
   * @returns 
   */
  static async DeleteUser(userId) {
    var verb = "DELETE"
    var path = this.SERVICE_ROUTE + "/" + this.USER_PATH + "?" + this.USER_ID_PATH + "=" + userId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }
}