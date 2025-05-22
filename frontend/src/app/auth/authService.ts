import { publicConfiguration } from "@/utils/configuration";
import axios, { isAxiosError } from "axios";
import { Result } from "@/utils/Result";

class AuthService {
    public static async SignUpWithPassword(
        email: string,
        password: string,
    ): Promise<Result<null, string>> {
        try {
            await axios.post(`${publicConfiguration.BACKEND_URL}/auth/register`, {
                email: email,
                password: password,
            }, {withCredentials: true});
            return {
                data: null,
                error: null,
            };
        } catch (err) {
            if (isAxiosError(err)) {
                return {
                    data: null,
                    error: err.response?.data?.message ?? "Unexpected error.",
                };
            }
            return {
                data: null,
                error: "Unexpected error.",
            };
        }
    }

    // Returns error message
    public static async SignInWithPassword(
        email: string,
        password: string,
    ): Promise<Result<null, string>> {
        try {
            await axios.post(
                `${publicConfiguration.BACKEND_URL}/auth/login`,
                {
                    email: email,
                    password: password,
                },
                { withCredentials: true },
            );
            return {
                data: null,
                error: null,
            };
        } catch (err) {
            if (isAxiosError(err)) {
                const msg = err.response?.data?.message ?? "Unexpected error";
                return {
                    data: null,
                    error: msg,
                };
            }
            return {
                data: null,
                error: "Unexpected error",
            };
        }
    }
}

export const authService = AuthService;
