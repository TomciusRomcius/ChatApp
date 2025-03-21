import { publicConfiguration } from "@/utils/configuration";
import axios from "axios";

class _ChatRoomService {
    async CreateChatRoom(
        name: string,
        memberIds: string[],
    ): Promise<string | null> {
        const res = await axios.post(
            `${publicConfiguration.BACKEND_URL}/chatroom`,
            {
                name: name,
                members: memberIds,
            },
            { withCredentials: true },
        );
        // TODO: may return error
        return res.data as string;
    }
}

const ChatRoomService = new _ChatRoomService();

export default ChatRoomService;
