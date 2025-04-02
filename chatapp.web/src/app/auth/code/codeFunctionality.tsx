"use client";

import OidcProviders from "@/services/oidc/oidcProviders";
import OidcProviderService from "@/services/oidc/oidcProviderService";
import { useSearchParams } from "next/navigation";
import { useEffect } from "react";

export function PageFunctionality() {
    const searchParams = useSearchParams();
    const authorizationCode = searchParams.get("code");
    const state = searchParams.get("state");

    const securityToken = state?.substring(15);

    useEffect(() => {
        if (!authorizationCode || !securityToken) {
            window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}/auth/sign-in}`;
            return;
        }

        // Send authorization code to the server which retrieves id_token and authenticates the user
        new OidcProviderService()
            .SignInUsingAuthorizationCode(
                OidcProviders.GOOGLE,
                authorizationCode,
                securityToken,
            )
            .then(() => {
                window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}`;
            })
            .catch(() => {
                window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}/auth/sign-in`;
            });
    });

    return <></>;
}
