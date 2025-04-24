import UserFriendsService from "@/services/userFriendsService";
import User from "../../_utils/user";
import { useContext } from "react";
import { AppState, AppStateContext } from "@/context/appStateContext";
import { FriendsContext } from "@/context/friendsContext";

interface FriendRequestsProps {
    friendRequests: User[];
}

export default function FriendRequests(props: FriendRequestsProps) {
    const { appState, setAppState } = useContext(AppStateContext);
    const { friends, setFriends } = useContext(FriendsContext);

    const onAcceptRequest = (userId: string) => {
        UserFriendsService.AcceptFriendRequest(userId).then(() => {
            const friendRequestUser = props.friendRequests.find(
                (fr) => fr.userId === userId,
            );
            if (!friendRequestUser) {
                alert("Something went wrong");
                return;
            }
            setFriends([...friends, friendRequestUser]);
            setAppState(AppState.DEFAULT);
        });
    };

    return (
        <>
            <h1 className="text-xl">Friend requests</h1>
            {props.friendRequests.map((user) => (
                <div key={user.userId} className="flex gap-4">
                    <small className="text-base">{user.username}</small>
                    <button onClick={() => onAcceptRequest(user.userId)}>
                        Accept
                    </button>
                </div>
            ))}
        </>
    );
}
