import React from "react";

import AuthCli from "../../sdk/AuthCli";
import UserCli from "../../sdk/UserCli";

export class SignupLayout extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      email: "",
      password: "",
      passwordConfirm: "",
      role: "",
      group: "",

      roles: [],
      groups: [],

      signupErrorMessage: null
    }
  }

  componentDidMount() {
    UserCli.GetRoles().then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ signupErrorMessage: response.substring(response.indexOf(' - ')) })
      } else {
        this.setState({ roles: response })
      }
    })

    UserCli.GetGroups().then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ signupErrorMessage: response.substring(response.indexOf(' - ')) })
      } else {
        this.setState({ groups: response })
      }
    })
  }

  handleInput = (event) => {
    this.setState({ [event.target.name]: event.target.value })
  }
  
  saveUser = () => {
    AuthCli.SignUpAsync({
      userEmail: this.state.email,
      password: this.state.password,
      passwordConfirmed: this.state.passwordConfirm,
      role: this.state.role,
      group: this.state.group
    }).then(response => {
      if (response) {
        this.setState({ signupErrorMessage: data.substring(data.indexOf(' - ')) })
      } else {
        console.log("Alta correcta")
        window.location.href = 'login'
      }
    })
  }

  render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-md-4 offset-md-4">
            <div className="card custom-card">
              <div class="card-header">
                <h4>Nuevo usuario</h4>
              </div>
              <div className="card-body">
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <input className="form-control custom-input" type="text" placeholder="Email" name="email" aria-label="input-email" value={this.state.email} onChange={this.handleInput} />
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <input className="form-control custom-input" type="password" placeholder="Contraseña" name="password" aria-label="input-password" value={this.state.password} onChange={this.handleInput} />
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <input className="form-control custom-input" type="password" placeholder="Confirmar contraseña" name="passwordConfirm" aria-label="input-passwordConfirm" value={this.state.passwordConfirm} onChange={this.handleInput} />
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <select className="form-select custom-input" aria-label="Default select" name="group" defaultValue="-1" value={this.state.group} onChange={this.handleInput}>
                      <option value="-1">Seelccione un grupo</option>
                      {this.state.groups && this.state.groups.map(group => {
                        return (<option key={group.id} value={group.name}>{group.name}</option>)
                      })}
                    </select>
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <select className="form-select custom-input" aria-label="Default select" name="role" defaultValue="-1" value={this.state.role} onChange={this.handleInput}>
                      <option value="-1">Seelccione un rol</option>
                      {this.state.roles && this.state.roles.map(role => {
                        return (<option key={role.id} value={role.name}>{role.name}</option>)
                      })}
                    </select>
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <button type="button" className="btn btn-primary custom-btn-primary float-end" onClick={this.saveUser}>Crear Usuario</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
      </React.Fragment>
    )
  }
}