import { CurrentUser } from "@/app/application/_utils/user";
import { publicConfiguration } from "@/utils/configuration";
import { Result } from "@/utils/Result";
import axios from "axios";
import { ApiErrorResponse } from "@/types";

interface UserInfoRequest {
    username: string;
}

class _UserService {
    async WhoAmI(): Promise<Result<CurrentUser | null, number>> {
        try {
            const res = await axios.get(
                `${publicConfiguration.BACKEND_URL}/user/whoami`,
                { withCredentials: true },
            );

            return {
                data: res.data as CurrentUser | null,
                error: 0,
                didSucceed: true,
            };
        } catch (err) {
            if (axios.isAxiosError(err)) {
                const response = err.response?.data as ApiErrorResponse;
                return {
                    data: null,
                    error: response.status,
                    didSucceed: false,
                };
            } else {
                return {
                    error: 0,
                    data: null,
                    didSucceed: false,
                };
            }
        }
    }

    async SetUserInfo(userInfo: UserInfoRequest): Promise<void> {
        await axios.post(
            `${publicConfiguration.BACKEND_URL}/user/user-info`,
            { ...userInfo },
            { withCredentials: true },
        );
    }
}

const UserService = new _UserService();

export default UserService;
