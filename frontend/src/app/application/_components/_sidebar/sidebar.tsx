import { useCallback, useContext, useEffect, useState } from "react";
import User from "../../_utils/user";
import SidebarUser from "./sidebarUser";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { CurrentChatContext } from "@/context/currentChatContext";
import { ChatRoom } from "@/types";
import UserFriendsService from "@/services/userFriendsService";
import Popup from "@/components/popup";
import FriendRequests from "@/app/application/_components/_popupElements/friendRequests";
import { createPortal } from "react-dom";
import { FriendsContext } from "@/context/friendsContext";
import ChatRoomService from "@/services/chatRoomService";
import CreateChatroom from "@/app/application/_components/_popupElements/createChatRoom";
import CurrentUserContext from "@/context/currentUserContext";
import SidebarChatRoom from "@/app/application/_components/_sidebar/sidebarChatRoom";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

interface SidebarProps {
    webSocket: WebSocket;
    friends: User[];
}

export default function Sidebar(props: SidebarProps) {
    const { appState, setAppState } = useContext(AppStateContext);
    const { setCurrentChat } = useContext(CurrentChatContext);
    const { friends, setFriends } = useContext(FriendsContext);
    const { currentUser } = useContext(CurrentUserContext);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);
    const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);

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

    const handleWsMessage = useCallback(
        (ev: MessageEvent) => {
            const msg = JSON.parse(ev.data);
            if (msg.type == "new-friend-request") {
                let user = msg.body as User;
                NotificationService.AddNotification(
                    `${user.username} send you a friend request!`,
                );
                setFriendRequests([...friendRequests, user]);
            } else if (msg.type == "accepted-friend-request") {
                let user = msg.body as User;
                NotificationService.AddNotification(
                    `${user.username} has accepted your friend request!`,
                );
                setFriends([...friends, user]);
            } else if (msg.type === "added-to-chat-room") {
                const chatRoom = msg.body as ChatRoom;
                NotificationService.AddNotification(
                    `You have been added to chat room: ${chatRoom.name}!`,
                );
                setChatRooms([...chatRooms, chatRoom]);
            }
        },
        [chatRooms],
    );

    const handleDeletedFriend = (deletedId: string) => {
        setFriends(friends.filter((f) => f.userId !== deletedId));
    };

    useEffect(() => {
        UserFriendsService.GetAllFriendRequests().then((requests) => {
            setFriendRequests(requests);
        });

        ChatRoomService.GetChatRooms().then((result) => {
            setChatRooms(result);
        });
    }, []);

    useEffect(() => {
        props.webSocket.addEventListener("message", handleWsMessage);

        return () => {
            props.webSocket.removeEventListener("message", handleWsMessage);
        };
    }, [handleWsMessage]);

    return (
        <>
            {appState == AppState.ACCEPT_FRIEND_REQUEST
                ? createPortal(
                      <Popup
                          onClose={() => setAppState(AppState.DEFAULT)}
                          className="flex flex-col gap-2"
                      >
                          <FriendRequests friendRequests={friendRequests} />
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

            <div className="col-span-2 flex flex-col items-start gap-12 bg-background-100 p-8 lg:col-span-1">
                <div className="flex w-full flex-col items-start gap-4">
                    <button onClick={onClickAddFriend}>Add friend</button>
                    <button onClick={onClickFriendRequests}>
                        Friend requests
                    </button>
                    <button onClick={onClickCreateChatRoom}>
                        Create a group
                    </button>
                </div>
                {/* Friends and group list */}
                <div className="flex h-full w-full flex-col items-start gap-4">
                    {friends.map((friend) => (
                        <button
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
                        </button>
                    ))}

                    {chatRooms.map((chatRoom) => (
                        <button
                            key={chatRoom.chatRoomId}
                            onClick={() =>
                                onSelectChatRoom(chatRoom.chatRoomId)
                            }
                        >
                            <SidebarChatRoom
                                key={chatRoom.chatRoomId}
                                chatRoomName={chatRoom.name}
                                chatRoomId={chatRoom.chatRoomId}
                            ></SidebarChatRoom>
                        </button>
                    ))}
                </div>
                <div className="h-[10%]">
                    <small className="text-base">{currentUser.username}</small>
                </div>
            </div>
        </>
    );
}
