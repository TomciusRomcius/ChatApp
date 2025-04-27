import { useContext } from "react";
import { CurrentChatContext } from "@/context/currentChatContext";

export interface SidebarChatEntry {
    name: string;
    chatId: string;
}

export default function SidebarChatEntry(props: SidebarChatEntry) {
    const { currentChat } = useContext(CurrentChatContext);

    const isCurrentChat = props.chatId === currentChat?.id;

    return (
        <div className="flex w-full gap-4">
            <small
                className={`${isCurrentChat ? "bg-background-200 text-text" : "text-textLighter"} w-full rounded-md p-2 text-sm`}
            >
                {props.name}
            </small>
        </div>
    );
}
