import axios from "axios";

export interface AuthenticationResponse {
    user?: string; // Jwt
}

export default class AuthenticationService {
    async Authenticate(
        authorizationCode: string,
    ): Promise<AuthenticationResponse> {
        const data = {
            authorizationCode: authorizationCode,
        };

        const res = await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL!}/auth/oidc`,
            data,
        );

        return res.data.user;
    }
}
