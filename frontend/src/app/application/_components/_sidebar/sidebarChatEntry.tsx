import React, { useContext } from "react";
import { CurrentChatContext } from "@/context/currentChatContext";
import CircleIcon from "@/components/icons/circleIcon";
import MoreIcon from "@/components/icons/moreIcon";

export interface SidebarChatEntry {
    name: string;
    chatId: string;
    type: "user" | "chatroom";
    onToggleContextMenu: (x: number, y: number) => void;
}

export default function SidebarChatEntry(props: SidebarChatEntry) {
    const { currentChat } = useContext(CurrentChatContext);

    const isCurrentChat = props.chatId === currentChat?.id;

    const handleClickMore = (e: React.MouseEvent<HTMLButtonElement>) => {
        props.onToggleContextMenu(e.clientX, e.clientY);
    };

    return (
        <div className="flex w-full items-center gap-4">
            <div>
                <CircleIcon
                    circleStroke="0"
                    size={8}
                    color={
                        props.type === "user"
                            ? "var(--color-accentLighter)"
                            : "var(--color-accent)"
                    }
                />
            </div>
            <small
                className={`${isCurrentChat ? "bg-background-200 text-text" : "text-textLighter"} w-full rounded-md p-2 text-sm transition hover:bg-background-200`}
            >
                {props.name}
            </small>
            <button onClick={handleClickMore}>
                <MoreIcon />
            </button>
        </div>
    );
}
