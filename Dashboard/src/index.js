import React from 'react';
import ReactDOM from 'react-dom';
import './css/bootstrap.css'
import './css/styles.css';
import './css/font-awesome.min.css';

import { createStore, applyMiddleware } from 'redux';
import thunkMiddleware from 'redux-thunk';

import App from './container/App';
import rootReducer from './reducers/rootReducer';

const store = createStore(
    rootReducer,
    window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__(),
    applyMiddleware(
      thunkMiddleware
    )
  );
  

ReactDOM.render(
    <App store={store} />, 
    document.getElementById('root')
);
