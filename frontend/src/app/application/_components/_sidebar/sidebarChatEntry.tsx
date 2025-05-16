import { useContext } from "react";
import { CurrentChatContext } from "@/context/currentChatContext";
import CircleIcon from "@/components/icons/circleIcon";

export interface SidebarChatEntry {
    name: string;
    chatId: string;
    type: "user" | "chatroom";
}

export default function SidebarChatEntry(props: SidebarChatEntry) {
    const { currentChat } = useContext(CurrentChatContext);

    const isCurrentChat = props.chatId === currentChat?.id;

    return (
        <div className="flex w-full items-center gap-4">
            <div>
                <CircleIcon
                    circleStroke="0"
                    size={8}
                    color={
                        props.type === "user"
                            ? "var(--color-accentLighter}"
                            : "var(--color-accent)"
                    }
                />
            </div>
            <small
                className={`${isCurrentChat ? "bg-background-200 text-text" : "text-textLighter"} w-full rounded-md p-2 text-sm transition hover:bg-background-200`}
            >
                {props.name}
            </small>
        </div>
    );
}
