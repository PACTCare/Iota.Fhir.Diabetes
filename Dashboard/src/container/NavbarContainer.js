import React, { Component } from 'react'
import { connect } from 'react-redux'
import Navbar from '../components/Navbar';
import { 
    selectPatient, 
    fetchPatientConditions, 
    fetchPatientObservations, 
    fetchPatientGlucoseSummary,
    fetchPatientImmunizations,
    fetchPatientDailyGlucoseOverview
} from '../actions/patientActions';
import { selectMenu } from '../actions/navigationActions';

class NavbarContainer extends Component {
    render() {
        return(
            <div>
                {
                    this.props.isLoading 
                    ? <img src='/loading.gif' alt='' />
                    : <Navbar patients={this.props.patients} 
                        onPatientClick={this.props.fetchPatient} 
                        selectedMenu={this.props.selectedMenu}
                        selectOverview={this.props.selectOverview}
                        selectMenuAction={this.props.selectMenu} />
                } 
            </div>
        );
    }
}

const mapStateToProps = (state) => {
    return {
        patients: state.Patients.patients,
        isLoading: state.Patients.isLoading,
        selectedMenu: state.Navigation.selectedMenu
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        fetchPatient: (patient) => {
            dispatch(selectPatient(patient));
            dispatch(fetchPatientGlucoseSummary(patient.resource.id, "", ""))
            dispatch(fetchPatientConditions(patient.resource.id))
            dispatch(fetchPatientObservations(patient.resource.id))
            dispatch(fetchPatientImmunizations(patient.resource.id))
        },
        selectMenu: (selectedMenu) => {
            dispatch(selectMenu(selectedMenu))
        },
        selectOverview: (patients) => {
            dispatch(selectMenu("Overview"))
            dispatch(fetchPatientDailyGlucoseOverview(patients))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(NavbarContainer)