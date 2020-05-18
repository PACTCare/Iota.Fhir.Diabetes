import 
{ 
    RECEIVE_PATIENTS, 
    REQUEST_PATIENTS,
    REQUEST_PATIENT_CONDITIONS, 
    RECEIVE_PATIENT_CONDITIONS,
    SELECT_PATIENT,
    REQUEST_PATIENT_OBSERVATIONS,
    RECEIVE_PATIENT_OBSERVATIONS,
    SELECT_PATIENT_SUBMENU,
    REQUEST_PATIENT_GLUCOSE_SUMMARY,
    RECEIVE_PATIENT_GLUCOSE_SUMMARY,
    ADD_PATIENT_DAILY_REPORT,
    REQUEST_PATIENT_IMMUNIZATIONS,
    RECEIVE_PATIENT_IMMUNIZATIONS,
    CLEAR_PATIENT_DAILY_GLUCOSE_OVERVIEW
} from "../constants/actionTypes";

const initialState = {
    patients: null,
    dailyReports: [],
    selectedPatient: null,
    isLoading: false,
    selectedSubmenu: "Summary",
    selectedPatientConditions: {
        isLoading: false,
        data: null
    },
    selectedPatientObservations: {
        isLoading: false,
        data: null
    },
    selectedPatientImmunizations: {
        isLoading: false,
        data: null
    },
    selectedPatientGlucoseSummary: {
        isLoading: false,
        data: null
    }
}
const patientsReducer = (state = initialState, action) => {
    switch (action.type) {
        case REQUEST_PATIENT_IMMUNIZATIONS:
            return Object.assign({}, state, {
                selectedPatientImmunizations: {
                    isLoading: true
                },
            });
        case RECEIVE_PATIENT_IMMUNIZATIONS:
            return Object.assign({}, state, {
                selectedPatientImmunizations: {
                    isLoading: false,
                    data: action.immunizations
                },
            });
        case REQUEST_PATIENT_GLUCOSE_SUMMARY:
            return Object.assign({}, state, {
                selectedPatientGlucoseSummary: {
                    isLoading: true
                },
            });
        case RECEIVE_PATIENT_GLUCOSE_SUMMARY:
            return Object.assign({}, state, {
                selectedPatientGlucoseSummary: {
                    isLoading: false,
                    data: action.summary
                },
            });
        case SELECT_PATIENT_SUBMENU:
            return Object.assign({}, state, {
                selectedSubmenu: action.submenu,
            });
        case RECEIVE_PATIENTS:
            return Object.assign({}, state, {
                patients: action.patients,
                isLoading: false
            });
        case REQUEST_PATIENTS:
            return Object.assign({}, state, {
                isLoading: true,
            });
        case SELECT_PATIENT:
            return Object.assign({}, state, {
                selectedPatient: action.patient.resource,
            });
        case REQUEST_PATIENT_CONDITIONS:
            return Object.assign({}, state, {
                selectedPatientConditions: {
                    isLoading: true
                },
            });
        case RECEIVE_PATIENT_CONDITIONS:
            return Object.assign({}, state, {
                selectedPatientConditions: {
                    isLoading: false,
                    data: action.conditions
                },
            });
        case REQUEST_PATIENT_OBSERVATIONS:
            return Object.assign({}, state, {
                selectedPatientObservations: {
                    isLoading: true
                },
            });
        case RECEIVE_PATIENT_OBSERVATIONS:
            return Object.assign({}, state, {
                selectedPatientObservations: {
                    isLoading: false,
                    data: action.observations
                },
            });
        case CLEAR_PATIENT_DAILY_GLUCOSE_OVERVIEW:
            return Object.assign({}, state, {
                dailyReports: []
            });
        case ADD_PATIENT_DAILY_REPORT:
            return Object.assign({}, state, {
                dailyReports: state.dailyReports.concat({ 
                    patientId: action.patientId, 
                    patientName: action.patientName,
                    report: action.report 
                }),
            });
        default:
            return state
    }
}

export default patientsReducer;