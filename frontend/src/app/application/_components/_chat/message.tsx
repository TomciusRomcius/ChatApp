import CurrentUserContext from "@/context/currentUserContext";
import TextMessage from "@/types";
import { useContext } from "react";

interface MessageProps {
    message: TextMessage;
}

export default function Message(props: MessageProps) {
    const { currentUser } = useContext(CurrentUserContext);

    const isSender = props.message.senderId == currentUser.id;

    return (
        <div
            className={`${isSender ? "self-end bg-sky-200" : "self-start bg-gray-200"} p-4 rounded-md flex flex-col gap-4`}
        >
            <div
                className={`flex flex-col gap-2 ${isSender ? "items-end" : "items-start"}`}
            >
                <small>{props.message.senderId}</small>
            </div>
            <p>{props.message.content}</p>
        </div>
    );
}
