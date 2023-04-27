/**
 * JavaScript version of the Auth service API
 */
export default class AuthCli {

  //
  // Path constants
  //
  static get AUTH_PATH() { return "auth" }
  static get SIGNUP_PATH() { return "signup" }
  static get LOGIN_PATH() { return "login" }
  static get LOGOUT_PATH() { return "logout" }
  static get SESSION_PATH() { return "session" }
  static get REFRESH_PATH() { return "refresh" }

  static get jwtToken() { return "jwtToken" }
  static get jwtRefresh() { return "jwtRefresh" }

  static get prefix() {return ('http://localhost:8001/')}


  /**
   * Admin is calling to register a new user providing the initial crentials
   * @param {*} credentials 
   * @returns 
   */
  static async SignUpAsync(credentials) {
    var verb = "POST"
    var path = this.AUTH_PATH + "/" + this.SIGNUP_PATH
    var body = credentials

    return this.Fetch(this.prefix + path, verb, body)
  }

  /**
   * Initiates a new session for user to work in the system
   * @param {*} credentials 
   * @returns 
   */
  static async LogInAsync(credentials) {
    var verb = "POST"
    var path = this.AUTH_PATH + "/" + this.LOGIN_PATH
    var body = credentials

    return fetch(this.prefix + path, this.getOptionsRequest(verb, body))
      .then(async response => {
        if (response.status === 204) return null
        if (response.status === 401) return response.text()
        if (response.status === 200) {
          await response.json().then(data => {
            if (data == null) {
              window.localStorage.removeItem(this.jwtToken)
              window.localStorage.removeItem(this.jwtRefresh)
            } else {
              window.localStorage.setItem(this.jwtToken, data.jwtToken)
              window.localStorage.setItem(this.jwtRefresh, data.refreshToken)
            }
          })
          return
        }
        return response.text()
      })
  }

  /**
   * Invalidates the current session so the token is no longer working.
   * @returns 
   */
  static async LogOutAsync() {
    var verb = "DELETE"
    var path = this.AUTH_PATH + "/" + this.LOGOUT_PATH
    var body = null

    return fetch(this.prefix + path, this.getOptionsRequest(verb, body))
      .then(response => response.text())
      .then(response => response.length ? JSON.parse(response) : null)
      .then(response => {
        window.localStorage.removeItem(this.jwtToken)
        window.localStorage.removeItem(this.jwtRefresh)
      })
  }

  /**
   * Provides information about current session.
   * @returns 
   */
  static async GetSessionInfoAsync() {
    var verb = "GET"
    var path = this.AUTH_PATH + "/" + this.SESSION_PATH
    var body = null

    return this.Fetch(this.prefix + path, verb, body)
  }
  
  /**
   * Request the Auth service to renew the session identifier in order to keep a previous login session alive
   * To be called when the JWT token has just expired and the user is still actively working.
   */
  static async RefreshSessionAsync() {
    var verb = "PUT"
    var path = this.AUTH_PATH + "/" + this.REFRESH_PATH
    var body = window.localStorage.getItem(this.jwtRefresh)

    return fetch(this.prefix + path, this.getOptionsRequest(verb, body))
      .then(response => response.status === 204 ? null : response.status === 200 ? response.json() : null)
      .then(response => {
        if (response == null) return;

        window.localStorage.setItem(this.jwtToken, response.jwtToken)
        window.localStorage.setItem(this.jwtRefresh, response.refreshToken)
      })
  }

  /**
   * Builds the request options for each API request.
   * @returns 
   */
  static getOptionsRequest(verb, body, multipart) {
    var jwtToken = window.localStorage.getItem(this.jwtToken)
    var culture = window.localStorage.getItem('culture')

    if (culture === null || culture === '') culture = 'en'

    if (multipart) {
      return {
        method: verb,
        headers: {
          'Authorization': 'Bearer ' + jwtToken,
          'culture': culture
        },
        body: body
      }
    }  
    else if (body === null && jwtToken === null) {
      return {
        method: verb,
        headers: {
          'Content-type': 'application/json',
          'culture': culture
        }
      }
    } else if (body === null && jwtToken !== null) {
      return {
        method: verb,
        headers: {
          'Content-type': 'application/json',
          'Authorization': 'Bearer ' + jwtToken,
          'culture': culture
        }
      }
    } else if (body !== null && jwtToken === null) {
      return {
        method: verb,
        headers: {
          'Content-type': 'application/json',
          'culture': culture
        },
        body: JSON.stringify(body)
      }
    } else if (body !== null && jwtToken !== null) {
      return {
        method: verb,
        headers: {
          'Content-type': 'application/json',
          'Authorization': 'Bearer ' + jwtToken,
          'culture': culture
        },
        body: JSON.stringify(body)
      }
    }
  }

  /**
   * To be used by other service clients in order to send API calls
   * Including the authentication header. Also, calls are automatically
   * retried when the authentication header expires.
   * @returns 
   */
  static async Fetch(path, verb, body, multipart) {
    return fetch(path, this.getOptionsRequest(verb, body, multipart))
      .then(async (response) => {
        if (response.status === 401) {
          console.log("Refreshing session...")
          await this.RefreshSessionAsync()
          console.log("Calling service...", verb, " - ", path)
          return fetch(path, this.getOptionsRequest(verb, body, multipart)).then((response) => {
            // Failed to refresh session, goes to login screen
            if (response.status === 401) {
              window.location.href = 'login'
            } else {
              return response
            }
          })
        } else {
          return response
        }
      })
      .then((response) => {
        if (response === 204) return null
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.indexOf("application/json") !== -1) return response.json()
        return response.text()
      })
  }
}