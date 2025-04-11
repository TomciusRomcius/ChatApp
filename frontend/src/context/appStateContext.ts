import { createContext } from "react";

export enum AppState {
    ADD_FRIEND,
    ACCEPT_FRIEND_REQUEST,
    CREATE_CHATROOM,
    DEFAULT,
}

export interface AppStateContextHook {
    appState: AppState;
    setAppState: (appState: AppState) => void;
}

export const AppStateContext = createContext<AppStateContextHook>(
    {} as AppStateContextHook,
); // Not ideal
