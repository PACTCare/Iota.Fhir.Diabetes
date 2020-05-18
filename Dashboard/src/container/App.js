import React, { Component } from 'react';
import { Provider } from 'react-redux';
import { fetchPatients } from '../actions/patientActions';
import { connect } from 'react-redux'

import NavbarContainer from './NavbarContainer';
import OverviewContainer from './OverviewContainer';
import PageContent from './PageContent';

class App extends Component {
  componentWillMount()
  {
    this.props.fetchPatients();
  }

  render() {
    return (
      <Provider store={this.props.store}>
        <div>
          <NavbarContainer />
          { this.props.selectedMenu === 'Overview' ? <OverviewContainer /> : <PageContent /> }
        </div>
      </Provider>
    );
  }
}

const mapStateToProps = (state) => {
  return {
      selectedMenu: state.Navigation.selectedMenu
  }
}

const mapDispatchToProps = (dispatch) => {
  return {
    fetchPatients: () => {
      dispatch(fetchPatients())
    }
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(App);
