import { CurrentChatContext } from "@/context/currentChatContext";
import { useCallback, useContext, useEffect, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import CurrentUserContext from "@/context/currentUserContext";
import ChatRoomMessagingService from "@/services/chatRoomMessagingService";
import ChatWindowRenderer from "./chatWindowRenderer";
import MessageSystem from "@/services/messageSystem";

interface ChatWindowProps {
    messageSystem: MessageSystem;
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

        props.messageSystem.AddNewMessage({
            senderId: currentUser.userId,
            receiverUserId: currentChat.id,
            content: content,
            createdAt: new Date().toISOString(),
        } as TextMessage);
    };

    const onUpdate = useCallback(() => {
        setMessages(structuredClone(props.messageSystem.GetMessages()));
    }, [props.messageSystem]);

    useEffect(() => {
        if (!currentChat) {
            return;
        }

        props.messageSystem.LoadMessages(currentChat).then(() => {});
        props.messageSystem.AddUpdateListener(onUpdate);
        return () => {
            props.messageSystem.RemoveListener(onUpdate);
        };
    }, [props.messageSystem, currentChat, onUpdate]);

    return (
        <ChatWindowRenderer sendMessage={onSendMessage} messages={messages} />
    );
}
