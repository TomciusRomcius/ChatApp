import { CurrentChatContext } from "@/context/currentChatContext";
import { useContext, useEffect, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import CurrentUserContext from "@/context/currentUserContext";
import ChatRoomMessagingService from "@/services/chatRoomMessagingService";
import ChatWindowRenderer from "./chatWindowRenderer";

export default function ChatWindow() {
    const { currentUser } = useContext(CurrentUserContext);
    const { currentChat } = useContext(CurrentChatContext);
    const [messages, setMessages] = useState<TextMessage[]>([]);

    const onSendMessage = (content: string) => {
        if (!currentChat?.id || !content) {
            return;
        }

        if (currentChat.type == "user") {
            UserMessagingService.SendMessageToFriend(currentChat.id, content);
        } else {
            ChatRoomMessagingService.SendMessage(currentChat.id, content);
            // TODO: set createdAt
        }
        // TODO: set createdAt
        setMessages([
            ...messages,
            {
                senderId: currentUser.id,
                receiverId: currentChat.id,
                content: content,
                createdAt: null,
            },
        ]);
    };

    useEffect(() => {
        if (!currentChat) {
            return;
        }

        if (currentChat.type == "user") {
            UserMessagingService.GetMessagesFromFriend(currentChat.id).then(
                (messages) => {
                    setMessages(messages);
                },
            );
        } else {
            ChatRoomMessagingService.GetMessages(currentChat.id, 0, 20).then(
                (messages) => {
                    setMessages(messages);
                },
            );
        }
    }, [currentChat]);

    return (
        <ChatWindowRenderer sendMessage={onSendMessage} messages={messages} />
    );
}
