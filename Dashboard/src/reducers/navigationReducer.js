import { SELECT_MENU } from "../constants/actionTypes";

const initialState = {
    selectedMenu: "Overview"
}

const navigationReducer = (state = initialState, action) => {
    switch (action.type) {
        case SELECT_MENU:
            return Object.assign({}, state, {
                selectedMenu: action.selectedMenu,
            });
        default:
            return state    
    }
}

export default navigationReducer;