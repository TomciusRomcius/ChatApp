import { useContext } from "react";
import User from "../../_utils/user";
import SidebarUser from "./sidebarUser";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { CurrentChatContext } from "@/context/currentChatContext";

interface SidebarProps {
    friends: User[];
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

    const onSelectChat = (userId: string) => {
        setCurrentChat({
            type: "user",
            id: userId,
        });
    };

    return (
        <div className="p-8 flex flex-col items-start gap-12 bg-background-100">
            <div className="w-full flex flex-col gap-4 items-start">
                <button onClick={onClickAddFriend}>Add friend</button>
                <button onClick={onClickFriendRequests}>Friend requests</button>
                <button onClick={onClickCreateChatRoom}>Create a group</button>
            </div>
            {/* Friends and group list */}
            <div className="w-full flex flex-col gap-4 items-start">
                {props.friends.map((friend) => (
                    <button
                        key={friend.userId}
                        onClick={() => onSelectChat(friend.userId)}
                    >
                        <SidebarUser
                            key={friend.userId}
                            username={friend.userName}
                        ></SidebarUser>
                    </button>
                ))}
            </div>
        </div>
    );
}
