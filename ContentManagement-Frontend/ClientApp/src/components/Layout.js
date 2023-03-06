import React, { Component } from 'react';

import UrjcLogo from '../images/urjc_logo.png'

export class Layout extends Component {
  static displayName = Layout.name;

  render() {
    return (
      <div>
        <nav className="navbar navbar-expand-lg bg-body-tertiary mb-5 pb-4 pt-4 custom-navbar">
          <div className="container">
            <a className="navbar-brand" href="/home">
              <img src={UrjcLogo} alt="" width={150} height={57}></img>
            </a>
            <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbar-main" aria-controls="navbar-main" aria-expanded="false" aria-label="Toggle navigation">
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbar-main">
              <div className="navbar-nav">
                <a className="nav-link custom-nav-link active" aria-current="page" href="/home">Inicio</a>
                <a className="nav-link custom-nav-link" href="/contents">Contenidos</a>
              </div>
            </div>
          </div>
        </nav>
        <div className="container mb-3">
          {this.props.children}
        </div>
      </div>
    );
  }
}
