import React, { useRef, useState } from "react";
import SidebarChatEntry from "@/app/application/_components/_sidebar/sidebarChatEntry";
import Modal from "@/components/Modal";

export interface SidebarChatRoomProps {
    chatRoomName: string;
    chatRoomId: string;
}

export default function SidebarChatRoom(props: SidebarChatRoomProps) {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const modalX = useRef<number>(0);
    const modalY = useRef<number>(0);

    const handleContextMenu = (e: React.MouseEvent<HTMLDivElement>) => {
        e.preventDefault();
        modalX.current = e.clientX;
        modalY.current = e.clientY;
        setIsModalOpen(true);
    };

    return (
        <div className="relative" onContextMenu={handleContextMenu}>
            {isModalOpen && (
                <Modal
                    x={modalX.current}
                    y={modalY.current}
                    onClose={() => setIsModalOpen(false)}
                >
                    <div className="flex flex-col gap-4 rounded-md bg-background-200 p-2"></div>
                </Modal>
            )}

            <SidebarChatEntry
                name={props.chatRoomName}
                chatId={props.chatRoomId}
            />
        </div>
    );
}
