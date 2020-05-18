import { combineReducers } from 'redux'
import patientsReducer from './patientsReducer'
import navigationReducer from './navigationReducer';

const rootReducer = combineReducers({
    Patients: patientsReducer,
    Navigation: navigationReducer
});

export default rootReducer;