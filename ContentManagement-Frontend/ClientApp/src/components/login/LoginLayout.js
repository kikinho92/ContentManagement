import React from "react";
import AuthCli from "../../sdk/AuthCli";

import { Login } from "./Login";
import { LoginConfirmed } from "./LoginConfirmed";

export class LoginLayout extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      isConfirmed: false
    }
  }

  componentDidMount() {
    this.checkCurrentSession()
  }

  checkCurrentSession = async () => {
    
    await AuthCli.GetSessionInfoAsync().then(async session => {
      if (!session) {
        this.setState({loginErrorMessage: session.substring(session.indexOf(' '))})
      } else {
        this.setState({ isConfirmed: true })
      }
      
    }) 
  }

  handleLogin = (isConfirmed) => {
    this.setState({ isConfirmed: isConfirmed })
    AuthCli.GetSessionInfoAsync().then(async session => {
      if (session !== null && session.toString().startsWith("[ERROR")) {
        this.setState({ loginErrorMessage: session.substring(session.indexOf(' ')) })
      } else {
        this.props.handleSession(session)
      }
    })
  }

  render() {
    return (
      <React.Fragment>
        {this.state.isConfirmed ?  <LoginConfirmed handleLogin={this.handleLogin}></LoginConfirmed> : <Login handleLogin={this.handleLogin}></Login>}
      </React.Fragment>
    )
    
  }
}