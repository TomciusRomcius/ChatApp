import { useContext, useRef, useState } from "react";
import User from "../../_utils/user";
import { AppState, AppStateContext } from "@/context/appStateContext";
import ChatRoomService from "@/services/chatRoomService";

interface CreateChatroomProps {
    friends: User[];
}

export default function CreateChatroom(props: CreateChatroomProps) {
    const { setAppState } = useContext(AppStateContext);
    const chatRoomNameRef = useRef<HTMLInputElement>(null);

    const [members, setMembers] = useState<User[]>([]);

    const potentialMembers = props.friends.filter(
        (f) => !members.find((m) => m.userId == f.userId),
    );

    const onCreateChatRoom = () => {
        const name = chatRoomNameRef.current?.value;

        if (name) {
            ChatRoomService.CreateChatRoom(
                name,
                members.map((member) => member.userId),
            );
            setAppState(AppState.DEFAULT);
        }
    };

    const addToChatroom = (userId: string) => {
        if (members.findIndex((m) => m.userId == userId) != -1) {
            return;
        }
        const user = props.friends.find((f) => f.userId == userId);
        if (user) {
            setMembers([...members, user]);
        }
    };

    return (
        <div className="flex flex-col gap-4 px-4">
            <h1 className="text-xl">Create chatroom</h1>
            <label>Chatroom name</label>
            <input ref={chatRoomNameRef} placeholder="Enter chatroom name" />

            <h2>Added members</h2>
            {members.length > 0
                ? members.map((member) => (
                      <p key={member.userId}>{member.userName}</p>
                  ))
                : null}
            <h2>Invite others</h2>

            <input placeholder="Enter friend's username" />
            {potentialMembers.map((friend) => (
                <button
                    key={friend.userId}
                    onClick={() => addToChatroom(friend.userId)}
                >
                    {friend.userName}
                </button>
            ))}
            <button onClick={onCreateChatRoom}>Create</button>
        </div>
    );
}
