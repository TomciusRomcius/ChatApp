import { useContext, useEffect, useState } from "react";
import ChatRoomService from "@/services/chatRoomService";
import User from "@/app/application/_utils/user";
import CurrentUserContext from "@/context/currentUserContext";
import { FriendsContext } from "@/context/friendsContext";
import AddMembers from "@/app/application/_components/_sidebar/_chatroom/addMembers";
import ToggleElement from "@/components/toggleElement";

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
            <h1 className="text-center text-2xl">Members</h1>
            {members.map((member) => (
                <div key={member.userId}>
                    <small className="w-full text-center">
                        {member.username}
                    </small>
                </div>
            ))}

            {currentUser.userId === props.adminUserId && (
                <>
                    <ToggleElement
                        containerClassName="flex flex-col gap-4"
                        buttonChildren={
                            <h1 className="text-xl">Add members</h1>
                        }
                        children={
                            <AddMembers
                                chatRoomId={props.chatRoomId}
                                currentMemberIds={members.map((m) => m.userId)}
                            />
                        }
                    />
                </>
            )}
        </div>
    );
}
