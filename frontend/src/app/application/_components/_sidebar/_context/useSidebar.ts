import { useCallback, useContext, useEffect, useState } from "react";
import User from "@/app/application/_utils/user";
import { ChatRoom } from "@/types";
import { FriendsContext } from "@/context/friendsContext";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

export function useSidebar(webSocket: WebSocket) {
    const { friends, setFriends } = useContext(FriendsContext);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);
    const [chatRooms, setChatRooms] = useState<ChatRoom[]>([]);

    const handleWsMessage = useCallback(
        (ev: MessageEvent) => {
            const msg = JSON.parse(ev.data);
            if (msg.type == "new-friend-request") {
                const user = msg.body as User;
                NotificationService.AddNotification(
                    `${user.username} send you a friend request!`,
                );
                setFriendRequests([...friendRequests, user]);
            } else if (msg.type == "accepted-friend-request") {
                const user = msg.body as User;
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
            } else if (msg.type === "removed-from-chat-room") {
                const chatRoom = chatRooms.find(
                    (cr) => cr.chatRoomId === msg.body.chatRoomId,
                );

                if (!chatRoom) {
                    console.error(
                        `Something is wrong. Received removed-from-chat-room 
                        message but chat room does not exist in state`,
                    );
                    return;
                }

                NotificationService.AddNotification(
                    `You have been removed from chatroom: ${chatRoom.name}`,
                );
                setChatRooms(
                    chatRooms.filter(
                        (cr) => cr.chatRoomId !== chatRoom.chatRoomId,
                    ),
                );
            } else if (msg.type === "removed-from-friends") {
                const userId = msg.body.id;
                setFriends(
                    friends.filter((friend) => friend.userId !== userId),
                );
            }
        },
        [chatRooms, friendRequests, friends, setFriends],
    );

    useEffect(() => {
        webSocket.addEventListener("message", handleWsMessage);

        return () => {
            webSocket.removeEventListener("message", handleWsMessage);
        };
    }, [handleWsMessage, webSocket]);

    return {
        friends,
        setFriends,
        friendRequests,
        setFriendRequests,
        chatRooms,
        setChatRooms,
    };
}
