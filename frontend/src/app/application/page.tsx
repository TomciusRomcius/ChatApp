"use client";

import { useEffect, useRef, useState } from "react";
import ClientSideApplication from "./clientSideApplication";
import UserService from "../../services/userService";
import { CurrentUser } from "./_utils/user";
import { useRouter } from "next/navigation";
import { publicConfiguration } from "@/utils/configuration";

export default function ApplicationPage() {
    const router = useRouter();
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>();

    const webSocketRef = useRef<WebSocket | null>(null);

    if (webSocketRef.current === null) {
        webSocketRef.current = new WebSocket(
            `${publicConfiguration.BACKEND_URL.replace("https", "wss")}/ws`,
        );
    }

    useEffect(() => {
        UserService.WhoAmI().then((result) => {
            // TODO: Not ideal, implement error codes
            if (result.didSucceed) {
                setCurrentUser(result.data);
            } else {
                if (result.error === "Account setup required!") {
                    router.replace("/auth/account-setup");
                } else {
                    router.replace("/auth/sign-in");
                }
            }
        });
    }, [router]);

    if (!currentUser) {
        return <h1>Loading</h1>;
    }

    return (
        <div className="flex min-h-screen w-screen gap-0">
            <ClientSideApplication
                currentUser={currentUser}
                webSocket={webSocketRef.current}
            ></ClientSideApplication>
        </div>
    );
}
