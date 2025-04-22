import { CurrentChatContext } from "@/context/currentChatContext";
import { useContext, useEffect, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import CurrentUserContext from "@/context/currentUserContext";
import ChatRoomMessagingService from "@/services/chatRoomMessagingService";
import ChatWindowRenderer from "./chatWindowRenderer";

interface ChatWindowProps {
    newMessages: TextMessage[];
}

export default function ChatWindow(props: ChatWindowProps) {
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
        // TODO: set createdAt properly
        setMessages([
            ...messages,
            {
                senderId: currentUser.id,
                receiverId: currentChat.id,
                content: content,
                createdAt: new Date(),
            } as TextMessage,
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

    useEffect(() => {
        setMessages([...messages, ...props.newMessages]);
        // TODO: temp fix
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [props.newMessages]);

    return (
        <ChatWindowRenderer sendMessage={onSendMessage} messages={messages} />
    );
}
