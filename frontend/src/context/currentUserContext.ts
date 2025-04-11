import { CurrentUser } from "@/app/application/_utils/user";
import { createContext } from "react";

interface CurrentUserWrapper {
    currentUser: CurrentUser;
    setCurrentUser: (currentUser: CurrentUser) => void;
}

const CurrentUserContext = createContext<CurrentUserWrapper>(
    {} as CurrentUserWrapper,
);

export default CurrentUserContext;
