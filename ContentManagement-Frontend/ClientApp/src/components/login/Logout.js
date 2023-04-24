import React from 'react'

import AuthCli from '../../sdk/AuthCli'

export class Logout extends React.Component{
  constructor(props) {
    super(props)
  }

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