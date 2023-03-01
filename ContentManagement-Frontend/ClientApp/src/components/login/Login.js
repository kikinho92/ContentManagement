import React from "react"; 
import AuthCli from "../../sdk/AuthCli";

export class Login extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      userEmail: '',

      userPassword: '',

      loginErrorMessage: null,

      isLoading: false
    }
  }

  onInputChange = (event) => {
    var inputId = event.target.id
    var inputValue = event.target.value

    this.setState({ [inputId]: inputValue })
  }

  doLogIn = () => {
    const { handleLogin } = this.props

    this.setState({ isLoading: true })
    
    AuthCli.LogInAsync({
      userEmail: this.state.userEmail,
      password: this.state.userPassword
    }).then(data => {
      if (data) {
        this.setState({ loginErrorMessage: data.substring(data.indexOf(' - ')) })
      } else {
        console.log("login correcto")
        handleLogin(true)
      }
    })
  }

  render() {
    return (
      <React.Fragment>
        <div className="container">
          <div className="row">
            <div className="col-md-4 offset-md-4">
              <div className="card">
                <h5 className="card-header"></h5>
                <div className="card-body">
                  <div className="mb-3">
                    <label htmlFor="userEmail" className="form-label">Email</label>
                    <input id="userEmail" type="email" className="form-control" placeholder="email@urjc.com" onChange={this.onInputChange} value={this.state.userEmail}></input>
                  </div>
                  <div className="mb-3">
                    <label htmlFor="userPassword" className="form-label">Password</label>
                    <input id="userPassword" type="password" className="form-control" placeholder="password" onChange={this.onInputChange} value={this.state.userPassword}></input>
                  </div>
                  <div className="mb-3">
                    <button type="button" className="btn btn-primary" onClick={this.doLogIn}>LogIn</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {this.state.loginErrorMessage !== null &&
          <div className="alert alert-danger" role="alert">
            {this.state.loginErrorMessage}
          </div>
        }

      </React.Fragment>
    )
  }
}