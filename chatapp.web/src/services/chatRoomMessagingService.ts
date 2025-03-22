import { publicConfiguration } from "@/utils/configuration";
import axios from "axios";

class _ChatRoomMessagingService {
    async GetMessages(
        chatRoomId: string,
        offset: number,
        numberOfMessages: number,
    ) {
        const res = await axios.get(
            `${publicConfiguration.BACKEND_URL}/chatroommessaging?ChatRoomId=${chatRoomId}&Offset=${offset}&NumberOfMessages=${numberOfMessages}`,
            { withCredentials: true },
        );

        return res.data;
    }

    async SendMessage(chatRoomId: string, content: string) {
        const res = await axios.post(
            `${publicConfiguration.BACKEND_URL}/chatroommessaging`,
            { chatRoomId: chatRoomId, content: content },
            { withCredentials: true },
        );

        return res.data;
    }
}

const ChatRoomMessagingService = new _ChatRoomMessagingService();

export default ChatRoomMessagingService;
