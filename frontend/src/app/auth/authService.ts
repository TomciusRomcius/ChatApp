import { publicConfiguration } from "@/utils/configuration";
import axios, { AxiosResponse } from "axios";

class AuthService {
    public static async SignUpWithPassword(
        email: string,
        password: string,
    ): Promise<Response> {
        const res = await fetch(`${publicConfiguration.BACKEND_URL}/register`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                email: email,
                password: password,
            }),
        });

        return res;
    }

    public static async SignInWithPassword(
        email: string,
        password: string,
    ): Promise<AxiosResponse> {
        const res = await axios.post(
            `${publicConfiguration.BACKEND_URL}/login?useCookies=true`,
            {
                email: email,
                password: password,
            },
            { withCredentials: true },
        );

        return res;
    }
}

export const authService = AuthService;
