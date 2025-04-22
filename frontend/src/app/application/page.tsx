"use client";

import { useEffect, useRef, useState } from "react";
import ClientSideApplication from "./clientSideApplication";
import UserService from "../../services/userService";
import { CurrentUser } from "./_utils/user";
import { useRouter } from "next/navigation";

export default function ApplicationPage() {
    const router = useRouter();
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>();

    const webSocketRef = useRef<WebSocket | null>(null);

    if (webSocketRef.current === null) {
        webSocketRef.current = new WebSocket("wss://localhost:5112/ws"); // Note: use "wss" for secure WebSocket
    }

    useEffect(() => {
        UserService.WhoAmI().then((result) => {
            if (result.errors.length > 0) {
                // TODO: Not ideal, implement error codes
                if (result.errors[0] === "Account setup required!") {
                    router.replace("/auth/account-setup");
                } else {
                    router.replace("/auth/sign-in");
                }
            } else {
                setCurrentUser(result.data);
            }
        });
    }, []);

    if (!currentUser) {
        return <h1>Loading</h1>;
    }

    return (
        <div className="grid min-h-screen w-screen grid-cols-6 grid-rows-1 gap-0">
            <ClientSideApplication
                currentUser={currentUser}
                webSocket={webSocketRef.current}
            ></ClientSideApplication>
        </div>
    );
}
