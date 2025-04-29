import { useContext, useEffect, useState } from "react";
import ChatRoomService from "@/services/chatRoomService";
import User from "@/app/application/_utils/user";
import CurrentUserContext from "@/context/currentUserContext";

interface MembersListProps {
    chatRoomId: string;
}

export default function MembersList(props: MembersListProps) {
    const { currentUser } = useContext(CurrentUserContext);
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
            <h1>Members</h1>
            {members.map((member) => (
                <div key={member.userId}>
                    <small>{member.username}</small>
                </div>
            ))}
        </div>
    );
}
