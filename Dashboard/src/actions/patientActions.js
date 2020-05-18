import 
{ 
    REQUEST_PATIENTS, 
    RECEIVE_PATIENTS, 
    SELECT_PATIENT,
    REQUEST_PATIENT_CONDITIONS, 
    RECEIVE_PATIENT_CONDITIONS,
    REQUEST_PATIENT_OBSERVATIONS,
    RECEIVE_PATIENT_OBSERVATIONS,
    SELECT_PATIENT_SUBMENU,
    REQUEST_PATIENT_GLUCOSE_SUMMARY,
    RECEIVE_PATIENT_GLUCOSE_SUMMARY,
    ADD_PATIENT_DAILY_REPORT,
    REQUEST_PATIENT_IMMUNIZATIONS,
    RECEIVE_PATIENT_IMMUNIZATIONS,
    CLEAR_PATIENT_DAILY_GLUCOSE_OVERVIEW
} from '../constants/actionTypes'

export const selectSubmenu = (submenu) => {
    return {
        type: SELECT_PATIENT_SUBMENU,
        submenu: submenu
    }
}

const requestPatients = () => {
    return {
        type: REQUEST_PATIENTS
    }
}

const receivePatients = (patients) => {
    return {
        type: RECEIVE_PATIENTS,
        patients: patients
    }
}

export const fetchPatients = () => dispatch => {
    dispatch(requestPatients());

    return fetch(('http://pactfhir.azurewebsites.net/api/fhir/Patient'), { 
                method: 'GET',
                mode: 'cors'
            }
        )
        .then(response => response.json())
        .then(json => {
            dispatch(receivePatients(json))
            dispatch(fetchPatientDailyGlucoseOverview(json))
        }
    );
}

export const fetchPatientDailyGlucoseOverview = (patients) => dispatch => {
    dispatch(clearPatientDailyGlucoseOverview())

    dispatch(addPatientDailyReport("FAKEPATIENT", "Jane Doe", fakeDailyReportJaneDoe))
    dispatch(addPatientDailyReport("FAKEPATIENT", "John Doe", fakeDailyReportJohnDoe))

    return patients.entry.forEach((patient) => {
        return fetch(('http://pactfhir.azurewebsites.net/api/glucose/daily/'.concat(patient.resource.id)), { 
                method: 'GET',
                mode: 'cors'
            }
        )
        .then(response => response.json())
        .then(json => {
            dispatch(addPatientDailyReport(patient.resource.id, patient.resource.name[0].family + ', ' + patient.resource.name[0].given[0], json))
        })
    });
}

const clearPatientDailyGlucoseOverview = () => {
    return {
        type: CLEAR_PATIENT_DAILY_GLUCOSE_OVERVIEW
    }
}

const addPatientDailyReport = (patientId, patientName, report) => {
    return {
        type: ADD_PATIENT_DAILY_REPORT,
        patientId: patientId,
        patientName: patientName,
        report: report
    }
}

const requestPatientConditions = () => {
    return {
        type: REQUEST_PATIENT_CONDITIONS,
    }
}

const receivePatientConditions = (conditions) => {
    return {
        type: RECEIVE_PATIENT_CONDITIONS,
        conditions: conditions
    }
}

const requestPatientObservations = () => {
    return {
        type: REQUEST_PATIENT_OBSERVATIONS,
    }
}

const receivePatientObservations = (observations) => {
    return {
        type: RECEIVE_PATIENT_OBSERVATIONS,
        observations: observations
    }
}

const requestPatientGlucoseSummary = () => {
    return {
        type: REQUEST_PATIENT_GLUCOSE_SUMMARY
    }
}

const receivePatientGlucoseSummary = (summary) => {
    return {
        type: RECEIVE_PATIENT_GLUCOSE_SUMMARY,
        summary: summary
    }
}

const requestPatientImmunizations = () => {
    return {
        type: REQUEST_PATIENT_IMMUNIZATIONS
    }
}

const receivePatientImmunizations = (immunizations) => {
    return {
        type: RECEIVE_PATIENT_IMMUNIZATIONS,
        immunizations: immunizations
    }
}

export const selectPatient = (patient) => {
    return {
        type: SELECT_PATIENT,
        patient: patient
    }
}

export const fetchPatientGlucoseSummary = (patientId, from, to) => dispatch => {
    dispatch(requestPatientGlucoseSummary());

    return fetch(('http://pactfhir.azurewebsites.net/api/glucose/summary/').concat(patientId).concat("?from=").concat(from).concat("&to=").concat(to), { 
                method: 'GET',
                mode: 'cors'
            }
        )
        .then(response => {
            console.log(response.ok)
            if (!response.ok) {
                alert(response.statusText)
            }

            return response.json()
        })
        .then(json => {
            dispatch(receivePatientGlucoseSummary(json))
        })
        .catch(error => {
            dispatch(receivePatientGlucoseSummary("NoData"))
        });
}

export const fetchPatientConditions = (patientId) => dispatch => {
    dispatch(fetchReferencedResource(patientId, 'Condition', requestPatientConditions, receivePatientConditions));
};

export const fetchPatientObservations = (patientId) => dispatch => {
    dispatch(fetchReferencedResource(patientId, 'Observation', requestPatientObservations, receivePatientObservations));
};

export const fetchPatientImmunizations = (patientId) => dispatch => {
    dispatch(fetchReferencedResource(patientId, 'Immunization', requestPatientImmunizations, receivePatientImmunizations));
}

const fetchReferencedResource = (reference, typeName, requestStart, callback) => dispatch => {
    dispatch(requestStart());

    return fetch(('http://pactfhir.azurewebsites.net/api/fhir/' + typeName + '?reference=').concat(reference), { 
                method: 'GET',
                mode: 'cors'
            }
        )
        .then(response => response.json())
        .then(json => {
            dispatch(callback(json))
        }
    );
};

const fakeDailyReportJohnDoe = [ 
    { 
       "averageGlucose":219.4062,
       "date":"2015-12-24T00:00:00.0000000+00:00",
       "indicator":"High"
    },
    { 
       "averageGlucose":233.7604,
       "date":"2015-12-25T00:00:00.0000000+00:00",
       "indicator":"Very High"
    },
    { 
       "averageGlucose":224.7326,
       "date":"2015-12-26T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":210.0069,
       "date":"2015-12-27T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":226.0486,
       "date":"2015-12-28T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":207.9375,
       "date":"2015-12-30T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":223.3194,
       "date":"2016-01-02T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":246.7778,
       "date":"2016-01-03T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":220.6477,
       "date":"2016-01-05T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":217.2222,
       "date":"2016-01-06T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":219.5833,
       "date":"2016-01-10T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":221.8368,
       "date":"2016-01-11T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":239.3295,
       "date":"2016-01-12T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":241.6215,
       "date":"2016-01-13T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":231.6285,
       "date":"2016-01-15T00:00:00.0000000+00:00",
       "indicator":"On Target"
    }
 ]

 const fakeDailyReportJaneDoe = [ 
    { 
       "averageGlucose":189.4062,
       "date":"2015-12-24T00:00:00.0000000+00:00",
       "indicator":"High"
    },
    { 
       "averageGlucose":167.7604,
       "date":"2015-12-25T00:00:00.0000000+00:00",
       "indicator":"Very High"
    },
    { 
       "averageGlucose":150.7326,
       "date":"2015-12-26T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":160.0069,
       "date":"2015-12-27T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":186.0486,
       "date":"2015-12-28T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":147.9375,
       "date":"2015-12-30T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":193.3194,
       "date":"2016-01-02T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":166.7778,
       "date":"2016-01-03T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":150.6477,
       "date":"2016-01-05T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":177.2222,
       "date":"2016-01-06T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":189.5833,
       "date":"2016-01-10T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":221.8368,
       "date":"2016-01-11T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":166.3295,
       "date":"2016-01-12T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":141.6215,
       "date":"2016-01-13T00:00:00.0000000+00:00",
       "indicator":"On Target"
    },
    { 
       "averageGlucose":131.6285,
       "date":"2016-01-15T00:00:00.0000000+00:00",
       "indicator":"On Target"
    }
 ]