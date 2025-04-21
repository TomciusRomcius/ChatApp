"use client"

import { useEffect, useState } from "react";
import ClientSideApplication from "./clientSideApplication";
import UserService from "../../services/userService";
import { CurrentUser } from "./_utils/user";
import { useRouter } from "next/navigation";

export default function ApplicationPage() {
    const router = useRouter();
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>();

    useEffect(() => {
        UserService.WhoAmI().then((result) => {
            if (result.errors.length > 0) {
                // TODO: Not ideal, implement error codes
                if (result.errors[0] === "Account setup required!") {
                    router.replace("/auth/account-setup");
                }
                else {
                    router.replace("/auth/sign-in");
                }
            }
            
            else {
                setCurrentUser(result.data);
            }
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
