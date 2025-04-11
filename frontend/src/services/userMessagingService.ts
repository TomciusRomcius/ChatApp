import TextMessage from "@/types";
import axios from "axios";

class _UserMessagingService {
    async GetMessagesFromFriend(userId: string): Promise<TextMessage[]> {
        const res = await axios.get(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/usermessage?UserId=${userId}`,
            { withCredentials: true },
        );

        return res.data ?? [];
    }

    async SendMessageToFriend(
        receiverId: string,
        content: string,
    ): Promise<string | null> {
        const res = await axios.post(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/usermessage`,
            { receiverId: receiverId, content: content },
            { withCredentials: true },
        );

        return res.data?.result;
    }
}

const UserMessagingService = new _UserMessagingService();

export default UserMessagingService;
