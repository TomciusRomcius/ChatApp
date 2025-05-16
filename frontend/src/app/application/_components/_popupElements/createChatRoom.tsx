import { useContext, useRef, useState } from "react";
import User from "../../_utils/user";
import { AppState, AppStateContext } from "@/context/appStateContext";
import ChatRoomService from "@/services/chatRoomService";
import { ChatRoom } from "@/types";
import CurrentUserContext from "@/context/currentUserContext";
import AccentButton from "@/components/accentButton";

interface CreateChatroomProps {
    friends: User[];
    onCreateChatRoom: (chatRoom: ChatRoom) => void;
}

export default function CreateChatroom(props: CreateChatroomProps) {
    const { currentUser } = useContext(CurrentUserContext);
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
            ).then((result) => {
                if (result.error !== null) {
                    alert("err");
                } else {
                    props.onCreateChatRoom({
                        name: name,
                        chatRoomId: result.data!,
                        adminUserId: currentUser.userId,
                    });
                }
            });
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

    const removeFromChatRoom = (userId: string) => {
        setMembers(members.filter((m) => m.userId !== userId));
    };

    return (
        <div className="flex flex-col gap-4 px-4">
            <h1 className="text-center text-2xl">Create chatroom</h1>
            <div className="flex flex-col gap-1">
                <label className="text-sm">Chatroom name</label>
                <input
                    ref={chatRoomNameRef}
                    placeholder="Enter chatroom name"
                />
            </div>
            <div className="h-[1px] bg-background-100"></div>

            <h2 className="text-center text-xl">Add members</h2>

            {potentialMembers.map((friend) => (
                <button
                    className="flex w-full items-center justify-between"
                    key={friend.userId}
                    onClick={() => addToChatroom(friend.userId)}
                >
                    {friend.username}
                    <small className="text-textLighter">Add</small>
                </button>
            ))}

            <h2 className="mt-8 text-base">Added members:</h2>
            {members.length > 0 ? (
                members.map((member) => (
                    <button
                        className="flex w-full items-center justify-between"
                        key={member.userId}
                        onClick={() => removeFromChatRoom(member.userId)}
                    >
                        {member.username}
                        <small className="text-textLighter">Remove</small>
                    </button>
                ))
            ) : (
                <small className="text-center text-textLighter">
                    No members, start by adding or create an empty group chat
                </small>
            )}
            <div className="h-[1px] bg-background-100"></div>
            <AccentButton onClick={onCreateChatRoom}>Create</AccentButton>
        </div>
    );
}
