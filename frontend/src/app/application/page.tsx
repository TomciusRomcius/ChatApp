"use client";

import UserFriendsService from "@/services/userFriendsService";
import { useEffect, useRef, useState } from "react";
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
import TextMessage, { ChatRoom } from "@/types";
import ChatRoomService from "@/services/chatRoomService";
import User, { CurrentUser } from "./_utils/user";

export default function ApplicationPage() {
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);
    const [currentChat, setCurrentChat] = useState<CurrentChat | null>(null);
    const [friends, setFriends] = useState<User[]>([]);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);
    const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);
    const [newMessages, setNewMessages] = useState<TextMessage[]>([]);
    const webSocket = useRef<WebSocket | null>(null);

    const handleWsMessage = (ev: MessageEvent) => {
        const msg = JSON.parse(ev.data);

        if (msg.Type == "user-message" || msg.Type == "chatroom-message") {
            const ent = msg.Body;
            const textMessage: TextMessage = {
                content: ent.Content,
                senderId: ent.SenderId,
                receiverId: ent.ReceiverId,
                chatRoomId: ent.ChatRoomId,
                createdAt: ent.CreatedAt,
            };
            setNewMessages([...newMessages, textMessage]);
        }
    };

    const webSocketSetup = (ws: WebSocket) => {
        ws.addEventListener("message", handleWsMessage);
    };

    useEffect(() => {
        UserService.WhoAmI().then((retrievedUser) => {
            setCurrentUser(retrievedUser);

            webSocket.current = new WebSocket(`https://localhost:5112/ws`);
            webSocketSetup(webSocket.current);
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

    const filteredChatNewMessages = !currentChat
        ? []
        : newMessages.filter((msg) => {
              if (currentChat.type == "user") {
                  return currentChat.id == msg.senderId;
              }

              if (currentChat.type == "chatroom") {
                  return currentChat.id == msg.chatRoomId;
              }
          });

    console.log(filteredChatNewMessages, newMessages, currentChat);

    return (
        <div className="gap-0 grid min-h-screen w-screen grid-cols-6 grid-rows-1">
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
                        <ChatWindow newMessages={filteredChatNewMessages} />
                    </CurrentChatContext.Provider>
                </AppStateContext.Provider>
            </CurrentUserContext.Provider>
        </div>
    );
}
