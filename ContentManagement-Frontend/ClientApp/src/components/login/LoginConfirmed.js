import React from "react";
import AuthCli from "../../sdk/AuthCli";

export class LoginConfirmed extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      isConfirmed: false,

      userInSession: null,

      loginErrorMessage: null
    }
  }

  componentDidMount() {
    this.checkCurrentSession()
  }

  checkCurrentSession = async () => {
    await AuthCli.GetSessionInfoAsync().then(async session => {

      if (session !== null && session.toString().startsWith("[ERROR")) {
        this.setState({ loginErrorMessage: session.substring(session.indexOf(' ')) })
      } else {
        this.setState({ userInSession: session.userEmail })
      }
      
    })
  }

  doLogOut = async () => {
    await AuthCli.LogOutAsync()

    this.props.handleLogin(false)
  }
    
  render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-md-4 offset-md-4">
            <div className="card custom-card">
              <div className="card-body">
                <h5 className="card-title">Login confirmado</h5>
                <p className="card-text">Usuario logeado como: {this.state.userInSession}.</p>
                <a href="#" className="btn btn-primary custom-btn-primary float-end" onClick={this.doLogOut}>Cerrar sesión</a>
              </div>
            </div>
          </div>
        </div>
      </React.Fragment>
    )
  }
}