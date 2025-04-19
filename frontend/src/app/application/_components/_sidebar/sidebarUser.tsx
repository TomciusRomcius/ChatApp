import { useContext } from "react";
import { CurrentChatContext } from "@/context/currentChatContext";

export interface SidebarUserProps {
    username: string;
    chatId: string;
}

export default function SidebarUser(props: SidebarUserProps) {
    const { currentChat } = useContext(CurrentChatContext);
    
    const isCurrentChat = props.chatId === currentChat?.id;
    
    return (
        <div className="w-full flex gap-4">
            <small className={`${isCurrentChat ? "text-text bg-background-200" : "text-textLighter"} p-2 rounded-md w-full text-base`}>{props.username}</small>
        </div>
    );
}
