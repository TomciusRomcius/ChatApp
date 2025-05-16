import React, { useContext, useRef, useState } from "react";
import SidebarChatEntry from "@/app/application/_components/_sidebar/sidebarChatEntry";
import Modal from "@/components/Modal";
import CurrentUserContext from "@/context/currentUserContext";
import { createPortal } from "react-dom";
import Popup from "@/components/popup";
import MembersList from "@/app/application/_components/_sidebar/_chatroom/membersList";
import ChatRoomService from "@/services/chatRoomService";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

export interface SidebarChatRoomProps {
    chatRoomName: string;
    chatRoomId: string;
    adminUserId: string;
    handleDeletedChatRoom: (chatRoomId: string) => void;
}

export default function SidebarChatRoom(props: SidebarChatRoomProps) {
    const { currentUser } = useContext(CurrentUserContext);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isMembersListOpen, setIsMembersListOpen] = useState(false);
    const modalX = useRef<number>(0);
    const modalY = useRef<number>(0);

    const isAdmin = currentUser.userId === props.adminUserId;

    const handleContextMenu = (e: React.MouseEvent<HTMLDivElement>) => {
        e.preventDefault();
        modalX.current = e.clientX;
        modalY.current = e.clientY;
        setIsModalOpen(true);
    };

    const onToogleMembersList = () => {
        setIsMembersListOpen(!isMembersListOpen);
    };

    const onDeleteChatRoom = () => {
        ChatRoomService.DeleteChatRoom(props.chatRoomId).then((result) => {
            if (result.error !== null) {
                NotificationService.AddNotification(
                    `Failed to delete chat room: ${result.error}`,
                );
            } else {
                props.handleDeletedChatRoom(props.chatRoomId);
                NotificationService.AddNotification(
                    `Successfully deleted chatroom: ${props.chatRoomName}`,
                );
            }
        });
    };

    const onLeaveChatRoom = () => {
        ChatRoomService.LeaveChatRoom(props.chatRoomId).then((result) => {
            if (result.error !== null) {
                NotificationService.AddNotification(
                    `Failed to delete chat room: ${result.error}`,
                );
            } else {
                props.handleDeletedChatRoom(props.chatRoomId);
            }
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
                    <div className="flex flex-col items-start gap-4 rounded-md bg-background-200 p-2">
                        <button onClick={onToogleMembersList}>Members</button>
                        {isAdmin && (
                            <button onClick={onDeleteChatRoom}>
                                Delete chat room
                            </button>
                        )}
                        {!isAdmin && (
                            <button onClick={onLeaveChatRoom}>
                                Leave chat room
                            </button>
                        )}
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
                type="chatroom"
                name={props.chatRoomName}
                chatId={props.chatRoomId}
            />
        </div>
    );
}
