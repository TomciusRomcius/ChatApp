import UserFriendsService from "@/services/userFriendsService";
import { useRef } from "react";

interface AddFriendProps {
    onSendFriendRequest: () => void;
}

export default function AddFriend(props: AddFriendProps) {
    const usernameRef = useRef<HTMLInputElement>(null);

    const onSendFriendRequest = () => {
        if (!usernameRef.current?.value) {
            throw new Error("Username ref is not defined");
        }

        UserFriendsService.SendFriendRequest(usernameRef.current.value).then(
            () => {
                props.onSendFriendRequest();
            },
        );
    };

    return (
        <>
            <h1 className="text-xl">Add a friend</h1>
            <label>Username</label>
            <input ref={usernameRef} placeholder="Enter username" />
            <button onClick={onSendFriendRequest}>Send friend request</button>
        </>
    );
}
