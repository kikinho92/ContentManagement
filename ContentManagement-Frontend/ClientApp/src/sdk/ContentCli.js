import AuthCli from "./AuthCli"
/**
 * JavaScript version of the Content service API
 */
export default class ContentCli {

  //
  // Path constants
  //
  static get SERVICE_ROUTE() { return "content" }

  static get PAGE_SIZE() { return "pageSize" }
  static get PAGE() { return "page" }
  static get CONTENT_LIST_PATH() { return "contentsList" }
  static get CONTENT_PATH() { return "contents" }
  static get TAG_ID_PATH() { return "tagId" }
  static get GROUP_ID_PATH() { return "groupId" }
  static get CONTENT_ID_PATH() { return "contentId" }
  static get TAG_PATH() { return "tag" }
  static get SEARCH_PATH() { return "search" }
  static get SEARCH_PARAM_PATH() { return "search" }
  static get UPLOAD_PATH() { return "upload" }
  static get USER_ID_PATH() { return "userid" }
  
  static get prefix() {return ('http://localhost:8003/')}
  /**
   * Provides detailed information about all the content. In case of tagId and group is not null, it will be provide the content that belong to the tag and group
   * @param {*} tagId Unique internal identifier of the tag
   * @param {*} groupId Unique internal identifier of the group
   * @returns 
   */
  static async GetContents(pageSize, page, tagId, groupId) {
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.CONTENT_LIST_PATH +
      "?" + this.PAGE_SIZE + "=" + pageSize +
      "&" + this.PAGE + "=" + page +
      "&" + this.TAG_ID_PATH + "=" + tagId +
      "&" + this.GROUP_ID_PATH + "=" + groupId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Provides detailed information about the content requested
   * @param {*} contentId Unique internal identifier of the content
   * @returns 
   */
  static async GetContent(contentId) {
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.CONTENT_PATH + "?" + this.CONTENT_ID_PATH + "=" + contentId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Save detailed information about content
   * @param {*} content Content entity that contains the information
   * @returns 
   */
  static async PostContent(content) {
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.CONTENT_PATH
    var body = content

    console.log("content", content)
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Modifies content which already exist in the db
   * @param {*} content Content entity that contains the new information
   * @param {*} contentId Unique internal identifier of the content which is going to be replaced
   * @returns 
   */
  static async PutContent(content, contentId) {
    var verb = "PUT"
    var path = this.SERVICE_ROUTE + "/" + this.CONTENT_PATH + "?" + this.CONTENT_ID_PATH + "=" + contentId
    var body = content
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Removes from the system the information about content.
   * @param {*} contentId Unique internal identifier of the content to be deleted
   * @returns 
   */
  static async DeleteContent(contentId) {
    var verb = "DELETE"
    var path = this.SERVICE_ROUTE + "/" + this.CONTENT_PATH + "?" + this.CONTENT_ID_PATH + "=" + contentId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Provides detailed information about all the tags. In case of group is not null, it will be provide the content that belong to the group.
   * @param {*} groupId Unique internal identifier of the group
   * @returns 
   */
  static async GetTags(groupId) {
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.TAG_PATH + "?" + this.GROUP_ID_PATH + "=" + groupId
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Save detailed information about tag
   * @param {*} tag Tag entity that contains the information
   * @returns 
   */
  static async PostTag(tag) {
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.TAG_PATH
    var body = tag
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Get contents filtered by a search.
   * @param {*} search partial or complete string to search in content info
   * @returns 
   */
  static async SearchContents(pageSize, page, search) {
    var verb = "GET"
    var path = this.SERVICE_ROUTE + "/" + this.SEARCH_PATH +
      "?" + this.PAGE_SIZE + "=" + pageSize +
      "&" + this.PAGE + "=" + page +
      "&" + this.SEARCH_PARAM_PATH + "=" + search
    var body = null
  
    return AuthCli.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Adds new content into collection of contents for the calling user by excel file.
   * @param {multi-part formData} body 
   * @returns 
   */
  static async UploadContents(userid, body) {
    var verb = "POST"
    var path = this.SERVICE_ROUTE + "/" + this.UPLOAD_PATH + "?" + this.USER_ID_PATH + "=" + userid
  
    return AuthCli.Fetch(this.prefix + path, verb, body, true)
  }
}