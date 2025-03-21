import { CurrentChatContext } from "@/context/currentChatContext";
import { useContext, useEffect, useRef, useState } from "react";
import TextMessage from "@/types";
import UserMessagingService from "@/services/userMessagingService";
import Message from "./message";
import CurrentUserContext from "@/context/currentUserContext";

export default function ChatWindow() {
    const { currentUser } = useContext(CurrentUserContext);
    const { currentChat } = useContext(CurrentChatContext);
    const [messages, setMessages] = useState<TextMessage[]>([]);
    const sendMessageRef = useRef<HTMLInputElement>(null);

    const onSendMessage = () => {
        const content = sendMessageRef.current?.value;

        if (currentChat?.id && content) {
            UserMessagingService.SendMessageToFriend(currentChat.id, content);
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
        }
    };

    useEffect(() => {
        if (currentChat) {
            UserMessagingService.GetMessagesFromFriend(currentChat.id).then(
                (messages) => {
                    setMessages(messages);
                },
            );
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
