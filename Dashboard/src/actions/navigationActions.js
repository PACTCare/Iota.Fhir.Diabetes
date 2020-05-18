import { SELECT_MENU } from "../constants/actionTypes";

export const selectMenu = (selectedMenu) => {
    return {
        type: SELECT_MENU,
        selectedMenu: selectedMenu
    }
}