import React, { Component } from 'react';
import { Redirect, Route, Switch } from 'react-router';
import { Layout } from './components/Layout';

import './custom.css'
import { LoginLayout } from './components/login/LoginLayout';
import { SignupLayout } from './components/signup/SignupLayout';
import { ContentLayout } from './components/contents/ContentLayout';
import { SekeerLayout } from './components/sekeer/SekeerLayout';
import { GroupsLayout } from './components/groups/GroupsLayout';
import { Logout } from './components/login/Logout';

export default class App extends Component {
  static displayName = App.name;

  constructor(props) {
    super(props)
    this.state = {
      session: null
    }
  }

  handleSession = (session) => {
    this.setState({ session: session })
  }

  render () {
    return (
      <Layout session={this.state.session}>
        <Switch>
          <Redirect exact from="/" to="/home"/>
          <Route path="/home" render={(props) => (<SekeerLayout {...props} />)} />
          <Route path="/login" render={(props) => (<LoginLayout {...props} handleSession={this.handleSession} />)} />
          <Route path="/logout" render={(props) => (<Logout {...props} />)} />
          <Route path="/signup" render={(props) => (<SignupLayout {...props} />)} />
          <Route path="/group" render={(props) => (<GroupsLayout {...props} />)} />
          <Route path="/contents" render={(props) => (<ContentLayout {...props} />)} />
        </Switch>
      </Layout>
    );
  }
}
