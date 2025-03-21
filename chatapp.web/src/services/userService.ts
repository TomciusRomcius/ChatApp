import { CurrentUser } from "@/app/application/_utils/user";
import { publicConfiguration } from "@/utils/configuration";
import axios from "axios";

class _UserService {
    async WhoAmI(): Promise<CurrentUser | null> {
        const res = await axios.get(
            `${publicConfiguration.BACKEND_URL}/user/whoami`,
            { withCredentials: true },
        );

        return (res.data ?? null) as CurrentUser | null;
    }
}

const UserService = new _UserService();

export default UserService;
