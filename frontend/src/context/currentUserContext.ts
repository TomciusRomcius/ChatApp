import { CurrentUser } from "@/app/application/_utils/user";
import { createContext } from "react";

interface CurrentUserWrapper {
    currentUser: CurrentUser;
}

const CurrentUserContext = createContext<CurrentUserWrapper>(
    {} as CurrentUserWrapper,
);

export default CurrentUserContext;
