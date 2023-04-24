import React from 'react'
import UserCli from '../../sdk/UserCli'

export class GroupsLayout extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      group: "",

      groupErrorMessage: null
    }
  }

  handleInput = (event) => {
    this.setState({ [event.target.name]: event.target.value })
  }

  saveGroup = () => {
    UserCli.PostGroup({
      id: null,
      name: this.state.group,
    }).then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ groupErrorMessage: response.substring(response.indexOf(' - ')) })
      } else {
        console.log("Alta de grupo correcta")
        window.location.href = 'home'
      }
    })
  }

  render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-md-4 offset-md-4">
            <div className="card custom-card">
              <div className="card-header">
                <h4>Nuevo grupo</h4>
              </div>
              <div className="card-body">
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <input className="form-control custom-input" type="text" placeholder="Grupo" name="group" aria-label="input-group" value={this.state.group} onChange={this.handleInput} />
                  </div>
                </div>
                <div className="row p-1">
                  <div className="col-xs-12 col-md-12">
                    <button type="button" className="btn btn-primary custom-btn-primary float-end" onClick={this.saveGroup}>Crear Grupo</button>
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