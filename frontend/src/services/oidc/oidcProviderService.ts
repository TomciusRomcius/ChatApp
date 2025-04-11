import axios from "axios";
import { OidcProvider } from "./oidcProviders";

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

    async SignInUsingAuthorizationCode(
        provider: OidcProvider,
        authorizationCode: string,
        securityToken: string,
    ): Promise<void> {
        const data = {
            authorizationCode: authorizationCode,
            provider: provider.providerName,
            securityToken: securityToken,
        };

        await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL!}/auth/oidc`,
            data,
            { withCredentials: true },
        );
    }
}
