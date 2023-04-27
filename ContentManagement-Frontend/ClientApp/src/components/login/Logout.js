import React from 'react'

import AuthCli from '../../sdk/AuthCli'

export class Logout extends React.Component{

  componentDidMount() {
    this.doLogOut()
  }

  doLogOut = async () => {
    AuthCli.LogOutAsync().then(response => {
      window.location.href = 'login'
    })
  }

  render() {
    return(<React.Fragment></React.Fragment>)
  }
}