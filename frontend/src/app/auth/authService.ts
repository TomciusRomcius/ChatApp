import { publicConfiguration } from "@/utils/configuration";
import axios, { isAxiosError } from "axios";
import { Result } from "@/utils/Result";
import { ApiErrorResponse } from "@/types";

class AuthService {
    public static async SignUpWithPassword(
        email: string,
        password: string,
    ): Promise<Result<null, string>> {
        try {
            await axios.post(
                `${publicConfiguration.BACKEND_URL}/auth/register`,
                {
                    email: email,
                    password: password,
                },
                { withCredentials: true },
            );
            return {
                data: null,
                error: "",
                didSucceed: true,
            };
        } catch (err) {
            if (isAxiosError(err)) {
                const response = err.response?.data as ApiErrorResponse;

                return {
                    data: null,
                    error: response.detail ?? "Unexpected error.",
                    didSucceed: false,
                };
            }
            return {
                data: null,
                error: "Unexpected error.",
                didSucceed: false,
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
                error: "",
                didSucceed: true,
            };
        } catch (err) {
            if (isAxiosError(err)) {
                const response = err.response?.data as ApiErrorResponse;
                return {
                    data: null,
                    error: response.detail,
                    didSucceed: false,
                };
            }
            return {
                data: null,
                error: "Unexpected error",
                didSucceed: false,
            };
        }
    }
}

export const authService = AuthService;
