import { createContext } from "react";

export interface CurrentChat {
    type: "user" | "chatroom";
    id: string;
}

export interface CurrentChatWrapper {
    currentChat: CurrentChat | null;
    setCurrentChat: (currentChat: CurrentChat) => void;
}

export const CurrentChatContext = createContext<CurrentChatWrapper>(
    {} as CurrentChatWrapper,
);
