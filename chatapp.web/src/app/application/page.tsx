"use client";

import UserFriendsService from "@/services/userFriendsService";
import { useEffect, useState } from "react";
import Sidebar from "./_components/_sidebar/sidebar";
import Popup from "@/components/popup";
import AddFriend from "./_components/_popupElements/addFriend";
import { AppState, AppStateContext } from "@/context/appStateContext";
import FriendRequests from "./_components/_popupElements/friendRequests";
import { CurrentChat, CurrentChatContext } from "@/context/currentChatContext";
import ChatWindow from "./_components/_chat/chatWindow";
import UserService from "@/services/userService";
import CurrentUserContext from "@/context/currentUserContext";
import CreateChatroom from "./_components/_popupElements/createChatRoom";
import { ChatRoom } from "@/types";
import ChatRoomService from "@/services/chatRoomService";
import User, { CurrentUser } from "./_utils/user";

export default function ApplicationPage() {
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);
    const [currentChat, setCurrentChat] = useState<CurrentChat | null>(null);
    const [friends, setFriends] = useState<User[]>([]);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);
    const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);

    useEffect(() => {
        UserService.WhoAmI().then((retrievedUser) => {
            setCurrentUser(retrievedUser);
        });

        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );

        UserFriendsService.GetAllFriendRequests().then((requests) => {
            setFriendRequests(requests);
        });

        ChatRoomService.GetChatRooms().then((result) => {
            setChatRooms(result);
        });
    }, []);

    console.log(`Current chat: ${currentChat?.id}`);

    if (!currentUser) {
        return "Getting current user";
    }

    return (
        <div className="w-screen min-h-screen grid grid-cols-6 grid-rows-1 gap-0">
            <CurrentUserContext.Provider
                value={{
                    currentUser: currentUser,
                    setCurrentUser: setCurrentUser,
                }}
            >
                <AppStateContext.Provider
                    value={{ appState: appState, setAppState: setAppState }}
                >
                    <CurrentChatContext.Provider
                        value={{
                            currentChat: currentChat,
                            setCurrentChat: setCurrentChat,
                        }}
                    >
                        {appState == AppState.ADD_FRIEND ? (
                            <Popup
                                onClose={() => setAppState(AppState.DEFAULT)}
                                className="flex flex-col gap-2"
                            >
                                <AddFriend
                                    onSendFriendRequest={() =>
                                        setAppState(AppState.DEFAULT)
                                    }
                                />
                            </Popup>
                        ) : null}

                        {appState == AppState.ACCEPT_FRIEND_REQUEST ? (
                            <Popup
                                onClose={() => setAppState(AppState.DEFAULT)}
                                className="flex flex-col gap-2"
                            >
                                <FriendRequests
                                    friendRequests={friendRequests}
                                />
                            </Popup>
                        ) : null}

                        {appState == AppState.CREATE_CHATROOM ? (
                            <Popup
                                onClose={() => setAppState(AppState.DEFAULT)}
                                className="flex flex-col gap-2"
                            >
                                <CreateChatroom friends={friends} />
                            </Popup>
                        ) : null}
                        <Sidebar friends={friends} chatRooms={chatRooms} />
                        <ChatWindow />
                    </CurrentChatContext.Provider>
                </AppStateContext.Provider>
            </CurrentUserContext.Provider>
        </div>
    );
}
