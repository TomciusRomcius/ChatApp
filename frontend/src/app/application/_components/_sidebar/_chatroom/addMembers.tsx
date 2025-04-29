import { useContext, useState } from "react";
import { FriendsContext } from "@/context/friendsContext";
import User from "@/app/application/_utils/user";
import ChatRoomService from "@/services/chatRoomService";

interface AddMembersProps {
    chatRoomId: string;
    currentMemberIds: string[];
}

export default function AddMembers(props: AddMembersProps) {
    const { friends } = useContext(FriendsContext);
    const [addedMembers, setAddedMembers] = useState<User[]>([]);

    let potentialMembers = friends.filter(
        (f) => !props.currentMemberIds.includes(f.userId),
    );
    potentialMembers = potentialMembers.filter(
        (f) => addedMembers.findIndex((am) => am.userId === f.userId) === -1,
    );

    const onSubmitAddMembers = () => {
        ChatRoomService.AddChatRoomMembers(
            props.chatRoomId,
            addedMembers.map((am) => am.userId),
        )
            .then((res) => {
                alert("WO");
            })
            .catch((err) => alert(err));
    };

    const onAddMemberToList = (user: User) => {
        setAddedMembers([...addedMembers, user]);
    };

    const onRemoveFromMembersList = (user: User) => {
        setAddedMembers(addedMembers.filter((am) => am.userId !== user.userId));
    };

    return (
        <>
            {potentialMembers.map((member) => (
                <button
                    onClick={() => onAddMemberToList(member)}
                    key={member.userId}
                >
                    {member.username}
                </button>
            ))}
            <h2>Added to list</h2>
            <div>
                {addedMembers.map((member) => (
                    <button
                        onClick={() => onRemoveFromMembersList(member)}
                        key={member.userId}
                    >
                        {member.username}
                    </button>
                ))}
            </div>
            <button onClick={onSubmitAddMembers}>Submit</button>
        </>
    );
}
