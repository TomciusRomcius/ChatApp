import { useCallback, useEffect, useRef, useState } from "react";
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
import MessageSystem from "@/services/messageSystem";

interface ClientSideApplicationProps {
    currentUser: CurrentUser;
    webSocket: WebSocket;
}

function generateMessageSystemKey(currentChat: CurrentChat) {
    return `${currentChat.type}.${currentChat.id}`;
}

function handleWsMessage(
    ev: MessageEvent,
    messageSystemMap: Map<string, MessageSystem>,
    friends: User[],
) {
    const msg = JSON.parse(ev.data);
    if (msg.type == "new-message") {
        const textMessage = msg.body as TextMessage;
        const isChatRoomMessage = !textMessage.receiverUserId;
        const chat: CurrentChat = {
            type: isChatRoomMessage ? "chatroom" : "user",
            id: isChatRoomMessage
                ? textMessage.chatRoomId!
                : textMessage.senderId!,
        };

        // If a message system is created for a chat room, append the text message to the messages array
        // and notify chatWindow of an update
        const messageSystem = messageSystemMap.get(
            generateMessageSystemKey(chat),
        );

        if (messageSystem) {
            messageSystem.AddNewMessage(textMessage);
        }

        let notification;
        if (textMessage.chatRoomId) {
            // TODO: add chatroom name
            notification = `New message in chatroom.`;
        } else {
            notification = `${friends.find((f) => f.userId == textMessage.senderId)?.username} sent you a message!`;
        }
        NotificationService.AddNotification(notification);
    }
}

export default function ClientSideApplication(
    props: ClientSideApplicationProps,
) {
    const currentUser = props.currentUser;
    const messageSystemMap = useRef(new Map<string, MessageSystem>());

    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);
    const [currentChat, setCurrentChat] = useState<CurrentChat | null>(null);
    const [friends, setFriends] = useState<User[]>([]);

    const onWebSocketMessage = useCallback(
        (ev: MessageEvent) => {
            handleWsMessage(ev, messageSystemMap.current, friends);
        },
        [messageSystemMap, friends],
    );

    useEffect(() => {
        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );
    }, []);

    useEffect(() => {
        props.webSocket.addEventListener("message", onWebSocketMessage);

        return () => {
            props.webSocket.removeEventListener("message", onWebSocketMessage);
        };
    }, [onWebSocketMessage, props.webSocket]);

    let currentMessageSystem = null;

    if (currentChat) {
        const messageSystemKey = generateMessageSystemKey(currentChat);
        currentMessageSystem = messageSystemMap.current.get(messageSystemKey);
        if (!currentMessageSystem) {
            currentMessageSystem = new MessageSystem(
                currentChat,
                messageSystemKey,
            );
            messageSystemMap.current.set(
                messageSystemKey,
                currentMessageSystem,
            );
        }
    }

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
                        <Sidebar webSocket={props.webSocket} />
                    </FriendsContext>
                    {currentChat && (
                        <ChatWindow messageSystem={currentMessageSystem!} />
                    )}
                    <NotificationsContainer />
                </CurrentChatContext.Provider>
            </AppStateContext.Provider>
        </CurrentUserContext.Provider>
    );
}
