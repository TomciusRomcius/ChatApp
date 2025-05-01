import { useCallback, useEffect, useState } from "react";
import Popup from "../../components/popup";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { CurrentChat, CurrentChatContext } from "@/context/currentChatContext";
import CurrentUserContext from "../../context/currentUserContext";
import UserFriendsService from "../../services/userFriendsService";
import TextMessage from "../../types";
import ChatWindow from "./_components/_chat/chatWindow";
import AddFriend from "./_components/_popupElements/addFriend";
import Sidebar from "./_components/_sidebar/sidebar";
import User, { CurrentUser } from "./_utils/user";
import { FriendsContext } from "@/context/friendsContext";
import NotificationsContainer from "@/app/application/_components/_popupElements/notificationsContainer";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

interface ClientSideApplicationProps {
    currentUser: CurrentUser;
    webSocket: WebSocket;
}

function filterNewMessages(
    currentChat: CurrentChat | null,
    newMessages: TextMessage[],
): TextMessage[] {
    if (!currentChat) return [];

    const filteredMessages: TextMessage[] = [];

    newMessages.forEach((msg) => {
        if (currentChat.type === "chatroom" && msg.chatRoomId) {
            filteredMessages.push(msg);
        }
        if (currentChat.type === "user" && !msg.chatRoomId) {
            filteredMessages.push(msg);
        }
    });

    return filteredMessages;
}

export default function ClientSideApplication(
    props: ClientSideApplicationProps,
) {
    const currentUser = props.currentUser;

    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);
    const [currentChat, setCurrentChat] = useState<CurrentChat | null>(null);
    const [friends, setFriends] = useState<User[]>([]);
    const [newMessages, setNewMessages] = useState<TextMessage[]>([]);

    const handleWsMessage = useCallback(
        (ev: MessageEvent) => {
            const msg = JSON.parse(ev.data);

            if (msg.type == "new-message") {
                const textMessage = msg.body as TextMessage;
                setNewMessages([...newMessages, textMessage]);
                let notification;
                if (textMessage.chatRoomId) {
                    // TODO: add chatroom name
                    notification = `New message in chatroom.`;
                } else {
                    notification = `${friends.find((f) => f.userId == textMessage.senderId)?.username} sent you a message!`;
                }
                NotificationService.AddNotification(notification);
            }
        },
        [currentUser, friends],
    );

    useEffect(() => {
        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );
    }, []);

    useEffect(() => {
        props.webSocket.addEventListener("message", handleWsMessage);

        return () => {
            props.webSocket.removeEventListener("message", handleWsMessage);
        };
    }, [handleWsMessage]);

    const filteredChatNewMessages = filterNewMessages(currentChat, newMessages);

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
                    <FriendsContext
                        value={{ friends: friends, setFriends: setFriends }}
                    >
                        <Sidebar
                            webSocket={props.webSocket}
                            friends={friends}
                        />
                    </FriendsContext>
                    <ChatWindow newMessages={filteredChatNewMessages} />
                    <NotificationsContainer />
                </CurrentChatContext.Provider>
            </AppStateContext.Provider>
        </CurrentUserContext.Provider>
    );
}
