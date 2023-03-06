import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';

import './custom.css'
import { LoginLayout } from './components/login/LoginLayout';
import { SignupLayout } from './components/signup/SignupLayout';
import { ContentLayout } from './components/contents/ContentLayout';
import { SekeerLayout } from './components/sekeer/SekeerLayout';

export default class App extends Component {
  static displayName = App.name;

  constructor(props) {
    super(props)
    this.state = {
      currentSession: null
    }
  }

  render () {
    return (
      <Layout>
        <Route exact path='/' component={SekeerLayout} />
        <Route exact path='/home' component={SekeerLayout} />
        <Route exact path='/login' component={LoginLayout} />
        <Route exact path='/signup' component={SignupLayout}/>
        <Route exact path='/contents' component={ContentLayout} />
      </Layout>
    );
  }
}
