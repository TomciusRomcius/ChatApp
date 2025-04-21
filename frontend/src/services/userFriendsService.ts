import User from "@/app/application/_utils/user";
import axios from "axios";

class _UserFriendsService {
    async GetAllFriends(): Promise<User[]> {
        const res = await axios.get(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/userfriend`,
            { withCredentials: true },
        );
        
        let result;
        if (Array.isArray(res.data)) {
            result = res.data.map((user: any) => ({
                userName: user.username,
                userId: user.userId
            } as User));
        }

        return result ?? [] as User[];
    }

    async GetAllFriendRequests(): Promise<User[]> {
        // TODO: add enum to avoid hard coded status
        const res = await axios.get(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/userfriend?status=0`,
            { withCredentials: true },
        );

        let result;
        if (Array.isArray(res.data)) {
            result = res.data.map((user: any) => ({
                userName: user.username,
                userId: user.userId
            } as User));
        }

        return result ?? [] as User[];
    }

    async SendFriendRequest(userId: string): Promise<void> {
        await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/userfriend/request`,
            { userId: userId },
            {
                withCredentials: true,
            },
        );
    }

    async AcceptFriendRequest(userId: string): Promise<void> {
        await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/userfriend/accept`,
            { userId: userId },
            {
                withCredentials: true,
            },
        );
    }
}

const UserFriendsService = new _UserFriendsService();

export default UserFriendsService;
