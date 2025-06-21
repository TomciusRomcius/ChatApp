import TextMessage from "@/types";
import Message from "./message";
import { useRef } from "react";
import SendIcon from "@/components/icons/sendIcon";

interface ChatWindowRendererProps {
    sendMessage: (content: string) => void;
    messages: TextMessage[];
}

export default function ChatWindowRenderer(props: ChatWindowRendererProps) {
    const sendMessageRef = useRef<HTMLInputElement>(null);

    const onSendMessage = () => {
        if (!sendMessageRef.current) {
            alert("Something went wrong");
            return;
        }
        const content = sendMessageRef.current.value;
        if (!content) {
            return;
        }

        sendMessageRef.current.value = "";
        props.sendMessage(content);
    };

    return (
        <div className="h-screen flex-1 px-4 py-16 lg:px-16 lg:py-32 xl:px-32 xl:py-8">
            <div className="flex h-full w-full flex-col">
                <div className="flex h-full flex-col gap-4 overflow-y-auto">
                    {props.messages.map((textMessage) => (
                        <Message
                            key={Math.random() * 100}
                            message={textMessage}
                        ></Message>
                    ))}
                </div>

                <div className="flex items-center justify-center overflow-hidden rounded-md border-[1px] border-background-200">
                    <div className="flex w-full items-center justify-between bg-background-100">
                        <input
                            ref={sendMessageRef}
                            className="full w-full p-4"
                            placeholder="Send message"
                        />
                        <button
                            onClick={onSendMessage}
                            className="mx-2 flex items-center justify-center rounded-md bg-accent p-2"
                        >
                            <SendIcon></SendIcon>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
