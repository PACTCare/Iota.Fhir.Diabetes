import React, { Component } from 'react'
import { connect } from 'react-redux'
import Overview from '../components/Overview';

class OverviewContainer extends Component {
    render() {
        return(
             <div id="page-wrapper" className="gray-bg">
                 <Overview reports={ this.props.reports } />
             </div>   
        );
    }
}

const mapStateToProps = (state) => {
    return {
        reports: state.Patients.dailyReports,
    }
}

export default connect(mapStateToProps)(OverviewContainer);