import axios from "axios";

export default class OidcAuthenticationService {
    async Authenticate(
        authorizationCode: string,
        securityToken: string,
    ): Promise<void> {
        const data = {
            authorizationCode: authorizationCode,
            provider: "google",
            securityToken: securityToken,
        };

        await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL!}/auth/oidc`,
            data,
            { withCredentials: true },
        );
    }
}
