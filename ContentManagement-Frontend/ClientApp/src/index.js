import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';

//Bootstrap css & js
/* import './bootstrap-5.2.3-dist/css/bootstrap.min.css'
import './bootstrap-5.2.3-dist/js/bootstrap.min.js' */

import 'bootstrap-icons/font/bootstrap-icons.css';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    <App />
  </BrowserRouter>,
  rootElement);

registerServiceWorker();

