import React, { Component } from 'react';

import OcaLogo from '../images/oca_logo.png'
import AuthCli from '../sdk/AuthCli';

export class Layout extends Component {
  static displayName = Layout.name;

  constructor(props) {
    super(props)
    this.state = {
      session: null
    }
  }

  componentDidMount() {
    if (this.props.session) {
      this.setState({ session: this.props.session })
    } 
    else {
      AuthCli.GetSessionInfoAsync().then(async session => {
        if (session) this.setState({ session: session })
      })
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps === this.props) return
    this.setState({ session: this.props.session })
  }

  render() {
    return (
      <div>

        <nav className="navbar navbar-expand-lg bg-body-tertiary mb-5 custom-navbar">
          <div className="container">
            <a className="navbar-brand" href="/home">
              <img src={OcaLogo} alt="" width={100}></img>
            </a>
            <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbar-main" aria-controls="navbar-main" aria-expanded="false" aria-label="Toggle navigation">
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse custom-responsive-nav" id="navbar-main">
              <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                <li className="nav-item">
                  <a className="nav-link custom-nav-link" aria-current="page" href="/home">Inicio</a>
                </li>
                <li className="nav-item">
                  <a className="nav-link custom-nav-link" href="/contents">Contenidos</a>
                </li>
                <li className="nav-item dropdown float-end">
                  <a className="nav-link dropdown-toggle custom-nav-link" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    Usuario<i className="bi bi-person-circle" style={{ marginLeft: "10px" }}></i>
                  </a>
                  <ul className="dropdown-menu custom-dropdown">
                    <li><a className="dropdown-item" href="/login">Log in</a></li>
                    <li><a className="dropdown-item" href="/logout">Log out</a></li>
                    <li><hr className="dropdown-divider" /></li>
                    {this.state.session && this.state.session.role === "SUPERUSER" &&
                      <li><a className="dropdown-item" href="/signup">Crear usuario</a></li>
                    }
                    {this.state.session && this.state.session.role === "SUPERUSER" &&
                      <li><a className="dropdown-item" href="/group">Crear grupo</a></li>
                    }
                  </ul>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        <div className="container" style={{ marginBottom: "100px" }}>
          {this.props.children}
        </div>

        <div className='custom-footer'>
          <div className='container'>
            <div className='row p-3'>
              <div className='col-md-4'>
                <p>© 2023 Universidad Rey Juan Carlos</p>
              </div>
              <div className='col-md-4'>

              </div>
              <div className='col-md-4'>

              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
