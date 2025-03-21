import TextMessage from "@/types";

interface MessageProps {
    message: TextMessage;
}

export default function Message(props: MessageProps) {
    return (
        <div className={`self-end flex flex-col gap-4`}>
            <div className="flex flex-col gap-2 items-end">
                <small>{props.message.senderId}</small>
                <small>{props.message.content}</small>
            </div>
            <p>Long LongLongLongLong message</p>
        </div>
    );
}
