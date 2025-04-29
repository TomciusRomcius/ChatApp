import { ChatRoom } from "@/types";
import { publicConfiguration } from "@/utils/configuration";
import axios, { isAxiosError } from "axios";
import User from "@/app/application/_utils/user";

interface CreateChatRoomResponse {
    chatRoomId: string;
}

class _ChatRoomService {
    async CreateChatRoom(
        name: string,
        memberIds: string[],
    ): Promise<Result<string, string>> {
        let result: Result<string, string> | null = null;

        try {
            const res = await axios.post(
                `${publicConfiguration.BACKEND_URL}/chatroom`,
                {
                    name: name,
                    members: memberIds,
                },
                { withCredentials: true },
            );

            const body = res.data as CreateChatRoomResponse;
            result = {
                data: body.chatRoomId,
                errors: [],
            };
        } catch (err) {
            if (isAxiosError(err)) {
                const msg = err.response?.data.message;
                result = {
                    data: null,
                    errors: [msg],
                };
            }

            result = {
                data: null,
                errors: ["Unexpected error"],
            };
        }

        return result;
    }

    async GetChatRooms(): Promise<ChatRoom[]> {
        const res = await axios.get(
            `${publicConfiguration.BACKEND_URL}/chatroom`,
            {
                withCredentials: true,
            },
        );
        // TODO: may return error
        return (res.data ?? []) as ChatRoom[];
    }

    async GetChatRoomMembers(chatRoomId: string): Promise<User[]> {
        const res = await axios.get(
            `${publicConfiguration.BACKEND_URL}/chatroom/members?chatRoomId=${chatRoomId}`,
            {
                withCredentials: true,
            },
        );

        return (res.data ?? []) as User[];
        // TODO: may return error
    }
}

const ChatRoomService = new _ChatRoomService();

export default ChatRoomService;
