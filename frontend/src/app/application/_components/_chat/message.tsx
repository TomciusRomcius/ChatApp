import CurrentUserContext from "@/context/currentUserContext";
import TextMessage from "@/types";
import { useContext } from "react";

interface MessageProps {
    message: TextMessage;
}

export default function Message(props: MessageProps) {
    const { currentUser } = useContext(CurrentUserContext);

    const isSender = props.message.senderId == currentUser.userId;

    const formattedDate = new Date(props.message.createdAt).toLocaleString(
        "en-US",
        {
            month: "long",
            weekday: "short",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            hour12: true,
        },
    );

    return (
        <div
            className={`${isSender ? "self-end bg-background-100" : "self-start bg-background-100"} flex flex-col gap-4 rounded-md p-4`}
        >
            <div
                className={`flex flex-col gap-2 ${isSender ? "items-end" : "items-start"}`}
            >
                <small className="text-textLighter">{formattedDate}</small>
            </div>
            <p>{props.message.content}</p>
        </div>
    );
}
