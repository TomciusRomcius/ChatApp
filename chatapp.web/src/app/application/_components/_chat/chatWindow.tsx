import { CurrentChatContext } from "@/context/currentChatContext";
import { useContext, useEffect, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import ChatView from "../chat/chatView";
import Message from "./message";

export default function ChatWindow() {
    const { currentChat } = useContext(CurrentChatContext);
    const [messages, setMessages] = useState<TextMessage[]>([]);

    useEffect(() => {
        if (currentChat) {
            UserMessagingService.GetMessagesFromFriend(currentChat.id).then(
                (messages) => {
                    setMessages(messages);
                },
            );

            UserMessagingService.SendMessageToFriend(currentChat.id, "Hello");
        }
    }, [currentChat]);
    console.log(messages);

    return (
        <div className="px-64 py-8 col-span-5 row-span flex flex-col">
            {messages.map((textMessage) => (
                <Message
                    key={Math.random() * 100}
                    message={textMessage}
                ></Message>
            ))}
        </div>
    );
}
