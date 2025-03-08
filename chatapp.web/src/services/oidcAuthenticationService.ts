import axios from "axios";

export default class OidcAuthenticationService {
    async Authenticate(authorizationCode: string): Promise<void> {
        const data = {
            authorizationCode: authorizationCode,
            provider: "google",
        };

        await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL!}/auth/oidc`,
            data,
            { withCredentials: true },
        );
    }
}
