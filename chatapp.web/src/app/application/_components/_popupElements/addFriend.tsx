import UserFriendsService from "@/services/userFriendsService";
import { useRef } from "react";

export default function AddFriend() {
    const userIdRef = useRef<HTMLInputElement>(null);

    const onSendFriendRequest = () => {
        if (!userIdRef.current?.value) {
            throw new Error("User id ref is not defined");
        }

        UserFriendsService.SendFriendRequest(userIdRef.current.value).then(
            () => {
                alert("Success");
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
