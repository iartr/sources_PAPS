import './App.css';
import 'react-chat-elements/dist/main.css';
import React from 'react';
import AuthService from './Services/AuthService';
import PrivateRoute from './Components/Auth/PrivateRoute';
import Home from './Components/Home';
import { Route, Switch, BrowserRouter as Router, Redirect } from 'react-router-dom';
import Header from './Components/Header';
import NotLoggedIn from './Components/Auth/NotLoggedIn';


function App() {  
  const isLoggedIn = AuthService.isLoggedIn();

  return (
  <div className="App">
    <Router>
      <Header title="123"></Header>
      <Switch>
        <Route exact path="/">
          <Redirect to="/home"/>
        </Route>
        <PrivateRoute isLoggedIn={isLoggedIn} path="/home" component={Home} />
        <Route path="/login" component={NotLoggedIn} />
      </Switch>
    </Router>
  </div>)
};


export default App;
