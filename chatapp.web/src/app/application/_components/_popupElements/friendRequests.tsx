import UserFriendsService from "@/services/userFriendsService";
import User from "../../_utils/user";

interface FriendRequestsProps {
    friendRequests: User[];
}

export default function FriendRequests(props: FriendRequestsProps) {
    const onAcceptRequest = (userId: string) => {
        UserFriendsService.AcceptFriendRequest(userId);
    };

    return (
        <>
            <h1 className="text-xl">Friend requests</h1>
            {props.friendRequests.map((user) => (
                <div key={user.userId} className="flex gap-4">
                    <small className="text-base">{user.userName}</small>
                    <button onClick={() => onAcceptRequest(user.userId)}>
                        Accept
                    </button>
                </div>
            ))}
        </>
    );
}
