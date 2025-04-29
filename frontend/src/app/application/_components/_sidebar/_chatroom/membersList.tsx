import { useContext, useEffect, useState } from "react";
import ChatRoomService from "@/services/chatRoomService";
import User from "@/app/application/_utils/user";
import CurrentUserContext from "@/context/currentUserContext";
import { FriendsContext } from "@/context/friendsContext";
import AddMembers from "@/app/application/_components/_sidebar/_chatroom/addMembers";

interface MembersListProps {
    chatRoomId: string;
    adminUserId: string;
}

export default function MembersList(props: MembersListProps) {
    const { currentUser } = useContext(CurrentUserContext);
    const { friends } = useContext(FriendsContext);
    const [members, setMembers] = useState<User[] | null>(null);

    useEffect(() => {
        ChatRoomService.GetChatRoomMembers(props.chatRoomId)
            .then((result) => {
                setMembers(
                    result.filter((m) => m.userId !== currentUser.userId),
                );
            })
            .catch(() => "err");
    }, []);

    if (members === null) {
        return <h1>Loading...</h1>;
    }

    return (
        <div className="flex flex-col gap-4">
            {currentUser.userId === props.adminUserId && (
                <>
                    <h1>Add members</h1>
                    <AddMembers
                        chatRoomId={props.chatRoomId}
                        currentMemberIds={members.map((m) => m.userId)}
                    />
                </>
            )}
            <h1>Members</h1>
            {members.map((member) => (
                <div key={member.userId}>
                    <small>{member.username}</small>
                </div>
            ))}
        </div>
    );
}
