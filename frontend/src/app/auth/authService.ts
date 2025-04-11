import { publicConfiguration } from "@/utils/configuration";

class AuthService {
    public static async SignUpWithPassword(
        username: string,
        email: string,
        password: string,
    ): Promise<Response> {
        console.log(username, email, password);

        const res = await fetch(`${publicConfiguration.BACKEND_URL}/register`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                username: username,
                email: email,
                password: password,
            }),
        });

        return res;
    }

    public static async SignInWithPassword(
        username: string,
        email: string,
        password: string,
    ): Promise<Response> {
        console.log(username, email, password);

        const res = await fetch(publicConfiguration.BACKEND_URL, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                username: username,
                email: email,
                password: password,
            }),
        });

        return res;
    }
}

export const authService = AuthService;
