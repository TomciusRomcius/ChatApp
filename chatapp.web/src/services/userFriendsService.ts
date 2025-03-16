import User from "@/app/application/_utils/user";
import axios from "axios";

class _UserFriendsService {
    async GetAllFriends(): Promise<User> {
        const res = await axios.get(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/userfriend`,
        );
        return res.data as User;
    }
}

const UserFriendsService = new _UserFriendsService();

export default UserFriendsService;
