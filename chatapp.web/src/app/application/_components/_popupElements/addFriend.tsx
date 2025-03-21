import UserFriendsService from "@/services/userFriendsService";
import { useRef } from "react";

interface AddFriendProps {
    onSendFriendRequest: () => void;
}

export default function AddFriend(props: AddFriendProps) {
    const userIdRef = useRef<HTMLInputElement>(null);

    const onSendFriendRequest = () => {
        if (!userIdRef.current?.value) {
            throw new Error("User id ref is not defined");
        }

        UserFriendsService.SendFriendRequest(userIdRef.current.value).then(
            () => {
                props.onSendFriendRequest();
            },
        );
    };

    return (
        <>
            <h1 className="text-xl">Add a friend</h1>
            <label>User id</label>
            <input ref={userIdRef} placeholder="Enter user id" />
            <button onClick={onSendFriendRequest}>Send friend request</button>
        </>
    );
}
