import React, { useContext, useRef, useState } from "react";
import SidebarChatEntry from "@/app/application/_components/_sidebar/sidebarChatEntry";
import Modal from "@/components/Modal";
import CurrentUserContext from "@/context/currentUserContext";
import { createPortal } from "react-dom";
import Popup from "@/components/popup";
import MembersList from "@/app/application/_components/_sidebar/_chatroom/membersList";

export interface SidebarChatRoomProps {
    chatRoomName: string;
    chatRoomId: string;
    adminUserId: string;
}

export default function SidebarChatRoom(props: SidebarChatRoomProps) {
    const { currentUser } = useContext(CurrentUserContext);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isMembersListOpen, setIsMembersListOpen] = useState(false);
    const modalX = useRef<number>(0);
    const modalY = useRef<number>(0);

    const handleContextMenu = (e: React.MouseEvent<HTMLDivElement>) => {
        e.preventDefault();
        modalX.current = e.clientX;
        modalY.current = e.clientY;
        setIsModalOpen(true);
    };

    const onToogleMembersList = () => {
        setIsMembersListOpen(!isMembersListOpen);
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
                        <button onClick={onToogleMembersList}>Members</button>
                        <button></button>
                    </div>
                </Modal>
            )}

            {isMembersListOpen &&
                createPortal(
                    <Popup onClose={onToogleMembersList}>
                        <MembersList
                            chatRoomId={props.chatRoomId}
                            adminUserId={props.adminUserId}
                        />
                    </Popup>,
                    document.body,
                )}

            <SidebarChatEntry
                name={props.chatRoomName}
                chatId={props.chatRoomId}
            />
        </div>
    );
}
