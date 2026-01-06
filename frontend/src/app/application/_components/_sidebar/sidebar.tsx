import { useContext, useEffect, useState } from "react";
import SidebarUser from "./sidebarUser";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { CurrentChatContext } from "@/context/currentChatContext";
import { ChatRoom } from "@/types";
import Popup from "@/components/popup";
import FriendRequests from "@/app/application/_components/_popupElements/friendRequests";
import { createPortal } from "react-dom";
import CreateChatroom from "@/app/application/_components/_popupElements/createChatRoom";
import CurrentUserContext from "@/context/currentUserContext";
import SidebarChatRoom from "@/app/application/_components/_sidebar/sidebarChatRoom";
import HamburgerMenu from "@/components/icons/hamburgerMenu";
import { useSidebar } from "@/app/application/_components/_sidebar/_context/useSidebar";
import UserFriendsService from "@/services/userFriendsService";

interface SidebarProps {
    webSocket: WebSocket;
}

export default function Sidebar(props: SidebarProps) {
    const [isOpen, setIsOpen] = useState(true);
    const { appState, setAppState } = useContext(AppStateContext);
    const { setCurrentChat } = useContext(CurrentChatContext);
    const { currentUser } = useContext(CurrentUserContext);
    const {
        friends,
        setFriends,
        friendRequests,
        setFriendRequests,
        chatRooms,
        setChatRooms,
    } = useSidebar(props.webSocket);

    const onClickAddFriend = () => {
        setAppState(AppState.ADD_FRIEND);
    };

    const onClickFriendRequests = () => {
        setAppState(AppState.ACCEPT_FRIEND_REQUEST);
    };

    const onClickCreateChatRoom = () => {
        setAppState(AppState.CREATE_CHATROOM);
    };

    const onSelectUserChat = (userId: string) => {
        setCurrentChat({
            type: "user",
            id: userId,
        });
    };

    const onSelectChatRoom = (chatRoomId: string) => {
        setCurrentChat({
            type: "chatroom",
            id: chatRoomId,
        });
    };

    const onCreateChatRoom = (chatRoom: ChatRoom) => {
        setChatRooms([...chatRooms, chatRoom]);
    };

    const handleDeletedFriend = (deletedId: string) => {
        setFriends(friends.filter((f) => f.userId !== deletedId));
    };

    const onAcceptFriendRequest = (friendId: string) => {
        setFriendRequests(friendRequests.filter((f) => f.userId !== friendId));
    };

    const handleDeletedChatRoom = (chatRoomId: string) => {
        setChatRooms(chatRooms.filter((cr) => cr.chatRoomId !== chatRoomId));
    };

    const onToggleSidebar = () => {
        setIsOpen(!isOpen);
    };

    useEffect(() => {
        UserFriendsService.GetAllFriendRequests().then(friendReqs => {
            setFriendRequests(friendReqs);
        });
    }, [setFriendRequests]);

    return (
        <>
            {appState == AppState.ACCEPT_FRIEND_REQUEST
                ? createPortal(
                      <Popup
                          onClose={() => setAppState(AppState.DEFAULT)}
                          className="flex flex-col gap-2"
                      >
                          <FriendRequests
                              friendRequests={friendRequests}
                              onAcceptFriendRequest={onAcceptFriendRequest}
                          />
                      </Popup>,
                      document.body,
                  )
                : null}

            {appState == AppState.CREATE_CHATROOM
                ? createPortal(
                      <Popup
                          onClose={() => setAppState(AppState.DEFAULT)}
                          className="flex flex-col gap-2"
                      >
                          <CreateChatroom
                              onCreateChatRoom={onCreateChatRoom}
                              friends={friends}
                          />
                      </Popup>,
                      document.body,
                  )
                : null}

            <div className={`fixed left-4 top-4`}>
                <button onClick={onToggleSidebar}>
                    <HamburgerMenu width={32} height={32} />
                </button>
            </div>

            <div
                className={`${!isOpen ? "hidden" : ""} w:1/2 flex h-screen flex-col items-start gap-12 overflow-y-auto border-r-[1px] border-background-200 bg-background-100 p-12 md:w-2/6 xl:w-1/6`}
            >
                <div className="flex w-full flex-col items-start gap-4">
                    <button
                        className="text-textLighter transition hover:text-text"
                        onClick={onClickAddFriend}
                    >
                        Add friend
                    </button>
                    <button
                        className="text-textLighter transition hover:text-text"
                        onClick={onClickFriendRequests}
                    >
                        Friend requests
                    </button>
                    <button
                        className="text-textLighter transition hover:text-text"
                        onClick={onClickCreateChatRoom}
                    >
                        Create a group
                    </button>
                </div>
                {/* Friends and group list */}
                <div className="flex h-full w-full flex-col items-start gap-4">
                    {friends.map((friend) => (
                        <div
                            className="w-full cursor-pointer"
                            key={friend.userId}
                            onClick={() => onSelectUserChat(friend.userId)}
                        >
                            <SidebarUser
                                key={friend.userId}
                                username={friend.username}
                                userId={friend.userId}
                                chatId={friend.userId}
                                onDeleteFriend={handleDeletedFriend}
                            ></SidebarUser>
                        </div>
                    ))}

                    {chatRooms.map((chatRoom) => (
                        <div
                            className="w-full cursor-pointer"
                            key={chatRoom.chatRoomId}
                            onClick={() =>
                                onSelectChatRoom(chatRoom.chatRoomId)
                            }
                        >
                            <SidebarChatRoom
                                key={chatRoom.chatRoomId}
                                chatRoomName={chatRoom.name}
                                chatRoomId={chatRoom.chatRoomId}
                                adminUserId={chatRoom.adminUserId}
                                handleDeletedChatRoom={handleDeletedChatRoom}
                            ></SidebarChatRoom>
                        </div>
                    ))}
                </div>
                <div className="h-[10%]">
                    <small className="text-base">{currentUser.username}</small>
                </div>
            </div>
        </>
    );
}
