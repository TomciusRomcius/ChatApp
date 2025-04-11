"use client"

import { useEffect, useState } from "react";
import ClientSideApplication from "./clientSideApplication";
import UserService from "../../services/userService";
import { CurrentUser } from "./_utils/user";

export default function ApplicationPage() {
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>();

    useEffect(() => {
        UserService.WhoAmI().then((user) => {
            setCurrentUser(user);
        }).catch(() => {

        })
    }, []);

    if (!currentUser) {
        return <h1>Loading</h1>
    }

    const webSocket = new WebSocket(`https://localhost:5112/ws`);

    return (
        <div className="gap-0 grid min-h-screen w-screen grid-cols-6 grid-rows-1">
            <ClientSideApplication currentUser={currentUser} webSocket={webSocket}></ClientSideApplication>
        </div>
    );
}
