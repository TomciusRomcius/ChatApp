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
        <div className="h-screen w-full md:col-span-4 md:px-8 md:py-32 lg:col-span-5 lg:px-64 lg:py-8">
            <div className="rows-10 grid h-full w-full">
                <div className="row-span-9 flex h-full flex-col gap-4 overflow-y-auto">
                    {props.messages.map((textMessage) => (
                        <Message
                            key={Math.random() * 100}
                            message={textMessage}
                        ></Message>
                    ))}
                </div>

                <div className="flex items-center justify-center">
                    <div className="flex w-full items-center justify-between rounded-md bg-background-100">
                        <input
                            ref={sendMessageRef}
                            className="h-auto w-full p-4"
                            placeholder="Send message"
                        />
                        <button onClick={onSendMessage} className="p-4">
                            Send
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
