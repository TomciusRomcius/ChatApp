import User from "@/app/application/_utils/user";
import { createContext } from "react";

export interface FriendsContextData {
    friends: User[];
    setFriends: (users: User[]) => void;
}

export const FriendsContext = createContext<FriendsContextData>(
    {} as FriendsContextData,
);
