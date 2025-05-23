import axios, { isAxiosError } from "axios";
import { OidcProvider } from "./oidcProviders";
import { Result } from "@/utils/Result";
import { ApiErrorResponse } from "@/types";
import { ApiErrorCodes } from "@/utils/apiErrors";

export default class OidcProviderService {
    RequestAuthorizationCode(
        provider: OidcProvider,
        clientId: string,
        securityToken: string,
    ) {
        const url = new URL(provider.oAuthUrl);

        url.searchParams.append("client_id", clientId);
        url.searchParams.append("response_type", "code");
        url.searchParams.append("scope", "openid email");
        url.searchParams.append("state", `security_token=${securityToken}`);
        url.searchParams.append(
            "redirect_uri",
            "https://localhost:3000/auth/code",
        );

        window.location.href = url.toString();
    }

    // <returns>Whether the user has setup public info</returns>
    async SignInUsingAuthorizationCode(
        provider: OidcProvider,
        authorizationCode: string,
        securityToken: string,
    ): Promise<Result<boolean, number>> {
        const data = {
            authorizationCode: authorizationCode,
            provider: provider.providerName,
            securityToken: securityToken,
        };

        try {
            const response = await axios.post(
                `${process.env.NEXT_PUBLIC_BACKEND_URL!}/auth/oidc`,
                data,
                { withCredentials: true },
            );
            
            return {
                data: response.data.isPublicInfoSetup,
                error: ApiErrorCodes.NO_ERROR,
                didSucceed: true,
            };
        }
        
        catch (error) {
            if (isAxiosError(error)) {
                const response = error.response?.data as ApiErrorResponse;
                return {
                    data: null,
                    error: response.status,
                    didSucceed: false,
                }
            }
            return {
                data: null,
                error: ApiErrorCodes.UNKNOWN,
                didSucceed: false,
            }
        }
    }
}
