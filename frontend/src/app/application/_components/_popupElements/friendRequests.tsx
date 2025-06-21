import UserFriendsService from "@/services/userFriendsService";
import User from "../../_utils/user";
import { useContext } from "react";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { FriendsContext } from "@/context/friendsContext";

interface FriendRequestsProps {
    friendRequests: User[];
    onAcceptFriendRequest: (friendId: string) => void;
}

export default function FriendRequests(props: FriendRequestsProps) {
    const { setAppState } = useContext(AppStateContext);
    const { friends, setFriends } = useContext(FriendsContext);

    const onClickAccept = (userId: string) => {
        UserFriendsService.AcceptFriendRequest(userId).then(() => {
            const friendRequestUser = props.friendRequests.find(
                (fr) => fr.userId === userId,
            );
            if (!friendRequestUser) {
                alert("Something went wrong");
                return;
            }
            setFriends([...friends, friendRequestUser]);
            props.onAcceptFriendRequest(userId);
            setAppState(AppState.DEFAULT);
        });
    };

    return (
        <>
            <h1 className="text-center text-xl">Friend requests</h1>
            {props.friendRequests.map((user) => (
                <div key={user.userId} className="flex gap-4">
                    <small className="text-base">{user.username}</small>
                    <button onClick={() => onClickAccept(user.userId)}>
                        Accept
                    </button>
                </div>
            ))}
        </>
    );
}
