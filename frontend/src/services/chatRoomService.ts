import { ChatRoom } from "@/types";
import { publicConfiguration } from "@/utils/configuration";
import axios, { isAxiosError } from "axios";
import User from "@/app/application/_utils/user";
import { Result } from "@/utils/Result";

interface CreateChatRoomResponse {
    chatRoomId: string;
}

class _ChatRoomService {
    async CreateChatRoom(
        name: string,
        memberIds: string[],
    ): Promise<Result<string, string>> {
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
            return {
                data: body.chatRoomId,
                error: null,
            };
        } catch (err) {
            if (isAxiosError(err)) {
                const msg = err.response?.data.message;
                return {
                    data: null,
                    error: msg,
                };
            }

            return {
                data: null,
                error: "Unexpected error",
            };
        }
    }

    async DeleteChatRoom(chatRoomId: string): Promise<Result<void, string>> {
        try {
            await axios.delete(
                `${publicConfiguration.BACKEND_URL}/chatroom?chatRoomId=${chatRoomId}`,
                { withCredentials: true },
            );

            return { data: null, error: null };
        } catch (err) {
            if (isAxiosError(err)) {
                const msg = err.response?.data.message;
                return {
                    data: null,
                    error: msg,
                };
            }

            return {
                data: null,
                error: "Unexpected error",
            };
        }
    }

    async LeaveChatRoom(chatRoomId: string): Promise<Result<void, string>> {
        try {
            await axios.post(
                `${publicConfiguration.BACKEND_URL}/chatroom/leave`,
                { chatRoomId: chatRoomId },
                { withCredentials: true },
            );

            return { data: null, error: null };
        } catch (err) {
            if (isAxiosError(err)) {
                const msg = err.response?.data.message;
                return {
                    data: null,
                    error: msg,
                };
            }
            return {
                data: null,
                error: "Unexpected error",
            };
        }
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

    async AddChatRoomMembers(
        chatRoomId: string,
        userIds: string[],
    ): Promise<void> {
        await axios.post(
            `${publicConfiguration.BACKEND_URL}/chatroom/members`,
            {
                chatRoomId: chatRoomId,
                userIds: userIds,
            },
            {
                withCredentials: true,
            },
        );
    }

    async RemoveChatRoomMembers(
        chatRoomId: string,
        userIds: string[],
    ): Promise<Result<void, string>> {
        try {
            await axios.post(
                `${publicConfiguration.BACKEND_URL}/chatroom/members/remove`,
                {
                    chatRoomId: chatRoomId,
                    userIds: userIds,
                },
                {
                    withCredentials: true,
                },
            );
            return { data: null, error: null };
        } catch (err) {
            if (isAxiosError(err)) {
                return {
                    data: null,
                    error: err.response?.data?.message ?? "Unexpected error",
                };
            }
            return {
                data: null,
                error: "Unexpected error",
            };
        }
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
