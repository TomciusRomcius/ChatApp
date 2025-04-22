import { useContext } from "react";
import User from "../../_utils/user";
import SidebarUser from "./sidebarUser";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { CurrentChatContext } from "@/context/currentChatContext";
import { ChatRoom } from "@/types";

interface SidebarProps {
    friends: User[];
    chatRooms: ChatRoom[];
}

export default function Sidebar(props: SidebarProps) {
    const { setAppState } = useContext(AppStateContext);
    const { setCurrentChat } = useContext(CurrentChatContext);

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

    console.log(props.chatRooms);

    return (
        <div className="col-span-2 flex flex-col items-start gap-12 bg-background-100 p-8 lg:col-span-1">
            <div className="flex w-full flex-col items-start gap-4">
                <button onClick={onClickAddFriend}>Add friend</button>
                <button onClick={onClickFriendRequests}>Friend requests</button>
                <button onClick={onClickCreateChatRoom}>Create a group</button>
            </div>
            {/* Friends and group list */}
            <div className="flex w-full flex-col items-start gap-4">
                {props.friends.map((friend) => (
                    <button
                        key={friend.userId}
                        onClick={() => onSelectUserChat(friend.userId)}
                    >
                        <SidebarUser
                            key={friend.userId}
                            username={friend.userName}
                            chatId={friend.userId}
                        ></SidebarUser>
                    </button>
                ))}

                {props.chatRooms.map((chatRoom) => (
                    <button
                        key={chatRoom.chatRoomId}
                        onClick={() => onSelectChatRoom(chatRoom.chatRoomId)}
                    >
                        <SidebarUser
                            key={chatRoom.chatRoomId}
                            username={chatRoom.name}
                            chatId={chatRoom.chatRoomId}
                        ></SidebarUser>
                    </button>
                ))}
            </div>
        </div>
    );
}
