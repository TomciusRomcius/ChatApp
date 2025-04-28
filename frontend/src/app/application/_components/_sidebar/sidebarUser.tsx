import React, { useRef, useState } from "react";
import SidebarChatEntry from "@/app/application/_components/_sidebar/sidebarChatEntry";
import Modal from "@/components/Modal";
import UserFriendsService from "@/services/userFriendsService";

export interface SidebarUserProps {
    username: string;
    userId: string;
    chatId: string;
    onDeleteFriend: (deletedId: string) => void;
}

export default function SidebarUser(props: SidebarUserProps) {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const modalX = useRef<number>(0);
    const modalY = useRef<number>(0);

    const handleContextMenu = (e: React.MouseEvent<HTMLDivElement>) => {
        e.preventDefault();
        modalX.current = e.clientX;
        modalY.current = e.clientY;
        setIsModalOpen(true);
    };

    const onRemoveFriend = () => {
        UserFriendsService.RemoveFriend(props.userId).then(() => {
            props.onDeleteFriend(props.userId);
        });
    };

    return (
        <div className="relative" onContextMenu={handleContextMenu}>
            {isModalOpen && (
                <Modal
                    x={modalX.current}
                    y={modalY.current}
                    onClose={() => setIsModalOpen(false)}
                >
                    <div className="flex flex-col gap-4 rounded-md bg-background-200 p-2">
                        <button onClick={onRemoveFriend}>Remove friend</button>
                    </div>
                </Modal>
            )}

            <SidebarChatEntry name={props.username} chatId={props.userId} />
        </div>
    );
}
