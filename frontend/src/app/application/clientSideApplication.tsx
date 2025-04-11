import { useState, useEffect } from "react";
import Popup from "../../components/popup";
import { AppState, AppStateContext } from "../../context/appStateContext";
import { CurrentChat, CurrentChatContext } from "../../context/currentChatContext";
import CurrentUserContext from "../../context/currentUserContext";
import ChatRoomService from "../../services/chatRoomService";
import UserFriendsService from "../../services/userFriendsService";
import TextMessage, { ChatRoom } from "../../types";
import ChatWindow from "./_components/_chat/chatWindow";
import AddFriend from "./_components/_popupElements/addFriend";
import CreateChatroom from "./_components/_popupElements/createChatRoom";
import FriendRequests from "./_components/_popupElements/friendRequests";
import Sidebar from "./_components/_sidebar/sidebar";
import User, { CurrentUser } from "./_utils/user";

interface ClientSideApplicationProps {
    currentUser: CurrentUser;
    webSocket: WebSocket;
}
export default function ClientSideApplication(props: ClientSideApplicationProps) {
    const currentUser = props.currentUser;

    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);
    const [currentChat, setCurrentChat] = useState<CurrentChat | null>(null);
    const [friends, setFriends] = useState<User[]>([]);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);
    const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);
    const [newMessages, setNewMessages] = useState<TextMessage[]>([]);

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
            console.log(currentUser.id, textMessage.senderId)

            if (textMessage.senderId != currentUser?.id) {
                setNewMessages([...newMessages, textMessage]);
            }
        }
    };

    useEffect(() => {
        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );

        UserFriendsService.GetAllFriendRequests().then((requests) => {
            setFriendRequests(requests);
        });

        ChatRoomService.GetChatRooms().then((result) => {
            setChatRooms(result);
        });

        props.webSocket.addEventListener("message", handleWsMessage);
    }, []);

    console.log(`Current chat: ${currentChat?.id}`);

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
        <CurrentUserContext.Provider
            value={{
                currentUser: currentUser,
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
    )
}