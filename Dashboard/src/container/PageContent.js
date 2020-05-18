import React, { Component } from 'react'
import { connect } from 'react-redux'

import { selectSubmenu, fetchPatientGlucoseSummary } from '../actions/patientActions';

import PatientDataPanel from '../components/panels/PatientDataPanel';
import PatientDataNavigation from '../components/panels/PatientDataNavigation';
import PatientGlucoseSummary from '../components/panels/PatientGlucoseSummary';
import ObservationPanel from '../components/panels/ObservationPanel';
import ConditionPanel from '../components/panels/ConditionPanel';
import ImmunizationPanel from '../components/panels/ImmunizationPanel';

class PageContent extends Component {
    render() {
        if (this.props.selectedPatient == null)
        {
            return(
                <div id="page-wrapper" className="gray-bg">
                </div>
            );
        }
        return(
            <div id="page-wrapper" className="gray-bg">
                <div className="row border-bottom">
                    <nav className="navbar navbar-static-top white-bg" style={{marginBottom: 0}}>
                        <div className="navbar-header col-lg-12">
                            <PatientDataPanel patientData={this.props.selectedPatient} />
                        </div>    
                    </nav>
                </div>
                <div>
                    <div className="row">
                        {
                            this.props.selectedPatient != null 
                            ? <div>
                                    <br />
                                        <PatientDataNavigation patientData={this.props.selectedPatient} selectMenu={this.props.changeSubmenu} />
                            </div>
                            : null
                        }
                    </div>
                </div>
                <div className="wrapper wrapper-content">
                    {
                        displaySummary(
                            this.props.selectedPatient,
                            this.props.selectedSubmenu, 
                            this.props.changeDate,
                            this.props.selectedPatientGlucoseSummary.isLoading, 
                            this.props.selectedPatientGlucoseSummary.data
                        )
                    }
                    {
                        displayClinical(
                            this.props.selectedSubmenu, 
                            this.props.selectedPatientConditions.isLoading,
                            this.props.selectedPatientConditions.data,
                            this.props.selectedPatientObservations.isLoading,
                            this.props.selectedPatientObservations.data,
                            this.props.selectedPatientImmunizations.isLoading,
                            this.props.selectedPatientImmunizations.data
                        )
                    }
                </div>
            </div>
        );
    }
}

const displaySummary = (patient, selectedSubmenu, changeDate, isLoading, summary) => {
    if (selectedSubmenu === "Summary") {
        return (
            <div>
                <div className="row">
                    {
                        isLoading === false && summary !== null 
                        ? <div>
                                <br />
                                    <PatientGlucoseSummary patient={patient} summary={summary} changeDate={changeDate} />
                        </div>
                        : <div>
                            <div className="col-lg-12 col-lg-offset-3">
                                <h2>Patient glucose summary is being prepared. This may take a few seconds.</h2>
                            </div>
                            <div className="col-lg-12 col-lg-offset-6"><img src='/loading.gif' alt='' /></div>
                        </div>
                    }
                </div>
            </div>
        );
    }
}

const displayClinical = (selectedSubmenu, conditionsLoading, conditions, observationsLoading, observations, immunizationsLoading, immunizations) => {
    if (selectedSubmenu === "Clinical") {
        return (
            <div>
                <div className="row">
                    <div>
                        <div className="col-lg-5">
                            {
                                conditionsLoading === false && conditions !== null
                                ? <ConditionPanel conditions={conditions} />
                                : <div className="col-lg-12"><img src='/loading.gif' alt='' /></div>
                            }
                        </div>
                        <div className="col-lg-1"></div>
                        <div className="col-lg-5">
                            {
                                observationsLoading === false && observations !== null
                                ? <ObservationPanel observations={observations} />
                                : <div className="col-lg-12"><img src='/loading.gif' alt='' /></div>
                            }
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div>
                        <div className="col-lg-5">
                            {
                                immunizationsLoading === false && immunizations !== null
                                ? <ImmunizationPanel immunizations={immunizations} />
                                : <div className="col-lg-12"><img src='/loading.gif' alt='' /></div>
                            }
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

const mapStateToProps = (state) => {
    return {
      selectedPatient: state.Patients.selectedPatient,
      selectedPatientConditions: state.Patients.selectedPatientConditions,
      selectedPatientObservations: state.Patients.selectedPatientObservations,
      selectedPatientImmunizations: state.Patients.selectedPatientImmunizations,
      selectedPatientGlucoseSummary: state.Patients.selectedPatientGlucoseSummary,
      selectedSubmenu: state.Patients.selectedSubmenu
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        changeSubmenu: (submenu) => {
            dispatch(selectSubmenu(submenu));
        },
        changeDate: (patientId, fromDate, toDate) => {
            dispatch(fetchPatientGlucoseSummary(patientId, fromDate, toDate));
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(PageContent)
