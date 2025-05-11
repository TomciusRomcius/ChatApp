import { CurrentUser } from "@/app/application/_utils/user";
import { publicConfiguration } from "@/utils/configuration";
import { Result } from "@/utils/Result";
import axios from "axios";

interface UserInfoRequest {
    username: string;
}

class _UserService {
    async WhoAmI(): Promise<Result<CurrentUser | null, string>> {
        let result: Result<CurrentUser | null, string> | null;

        try {
            const res = await axios.get(
                `${publicConfiguration.BACKEND_URL}/user/whoami`,
                { withCredentials: true },
            );

            result = {
                data: res.data as CurrentUser | null,
                error: null,
            };
        } catch (err) {
            if (axios.isAxiosError(err)) {
                result = {
                    error: err.response?.data?.toString(),
                    data: null,
                };
            } else {
                result = {
                    error: "Unexpected error occurred.",
                    data: null,
                };
            }
        }

        return result;
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
