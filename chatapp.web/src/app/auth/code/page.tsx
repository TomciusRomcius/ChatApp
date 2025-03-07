"use client";

import AuthenticationService, {
    AuthenticationResponse,
} from "@/services/authenticationService";
import { useSearchParams } from "next/navigation";
import { useEffect } from "react";

export default function AuthorizationCodeCallbackPage() {
    const searchParams = useSearchParams();
    const authorizationCode = searchParams.get("code");

    const onAuthenticationResponse = (response: AuthenticationResponse) => {
        if (response) {
            window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}`;
        } else {
            window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}/auth/sign-in}`;
        }
    };

    useEffect(() => {
        if (!authorizationCode) {
            window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}/auth/sign-in}`;
            return;
        }

        // Send authorization code to the server which retrieves id_token and authenticates the user
        new AuthenticationService()
            .Authenticate(authorizationCode)
            .then((response) => {
                onAuthenticationResponse(response);
            });
    }, []);

    return <h1>Loading</h1>;
}
