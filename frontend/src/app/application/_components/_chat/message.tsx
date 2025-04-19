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
            className={`${isSender ? "self-end bg-background-100" : "self-start bg-background-100"} p-4 rounded-md flex flex-col gap-4`}
        >
            <div
                className={`flex flex-col gap-2 ${isSender ? "items-end" : "items-start"}`}
            >
                <small className="text-textLighter">TODO: add date</small>
            </div>
            <p>{props.message.content}</p>
        </div>
    );
}
