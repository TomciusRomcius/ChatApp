import TextMessage from "@/types";
import Message from "./message";
import { useRef } from "react";

interface ChatWindowRendererProps {
    sendMessage: (content: string) => void;
    messages: TextMessage[];
}

export default function ChatWindowRenderer(props: ChatWindowRendererProps) {
    const sendMessageRef = useRef<HTMLInputElement>(null);

    const onSendMessage = () => {
        const content = sendMessageRef.current?.value;
        if (!content) {
            return;
        }

        props.sendMessage(content);
    };

    return (
        <div className="w-full h-screen px-64 py-8 col-span-5">
            <div className="w-full h-full grid rows-10">
                <div className="flex h-full overflow-y-auto flex-col gap-4 row-span-9">
                    {props.messages.map((textMessage) => (
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
