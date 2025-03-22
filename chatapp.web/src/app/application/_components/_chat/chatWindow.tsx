import { CurrentChatContext } from "@/context/currentChatContext";
import { useContext, useEffect, useRef, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import Message from "./message";
import CurrentUserContext from "@/context/currentUserContext";
import ChatRoomMessagingService from "@/services/chatRoomMessagingService";

export default function ChatWindow() {
    const { currentUser } = useContext(CurrentUserContext);
    const { currentChat } = useContext(CurrentChatContext);
    const [messages, setMessages] = useState<TextMessage[]>([]);
    const sendMessageRef = useRef<HTMLInputElement>(null);

    const onSendMessage = () => {
        const content = sendMessageRef.current?.value;
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
        if (currentChat) {
            if (currentChat.type == "user") {
                UserMessagingService.GetMessagesFromFriend(currentChat.id).then(
                    (messages) => {
                        setMessages(messages);
                    },
                );
            } else {
                ChatRoomMessagingService.GetMessages(
                    currentChat.id,
                    0,
                    20,
                ).then((messages) => {
                    setMessages(messages);
                });
            }
        }
    }, [currentChat]);

    return (
        <div className="w-full h-screen px-64 py-8 col-span-5">
            <div className="w-full h-full grid rows-10">
                <div className="flex h-full overflow-y-auto flex-col gap-4 row-span-9">
                    {messages.map((textMessage) => (
                        <Message
                            key={Math.random() * 100}
                            message={textMessage}
                        ></Message>
                    ))}
                </div>

                <div className="w-full px-4 py-2 flex items-center justify-between bg-gray-200">
                    <input
                        ref={sendMessageRef}
                        className="h-full"
                        placeholder="Send message"
                    />
                    <button onClick={onSendMessage}>Send</button>
                </div>
            </div>
        </div>
    );
}
